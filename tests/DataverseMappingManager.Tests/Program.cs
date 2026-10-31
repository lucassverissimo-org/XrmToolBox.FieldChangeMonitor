using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.Windows.Forms;
using System.Xml.Linq;
using LucasVerissimo.XrmToolBox.DataverseMappingManager;
using LucasVerissimo.XrmToolBox.DataverseMappingManager.Models;
using LucasVerissimo.XrmToolBox.DataverseMappingManager.Services;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Query;
using XrmToolBox.Extensibility;

internal static class Program
{
    private static int assertions;

    [STAThread]
    private static int Main()
    {
        try
        {
            Application.EnableVisualStyles();
            Rules();
            SaveAndRetry();
            SwapAndRollback();
            Conflict();
            UncertainSave();
            DesignerLoading();
            SolutionComponents();
            ExportVerification();
            NativeExportOmission();
            SolutionSelection();
            Console.WriteLine("Starting UI checks");
            UserInterface();
            MappingCheckSynchronization();
            EditingAndRefresh();
            ReviewExecution();
            ReviewSolutionAction();
            SaveProgress();
            Console.WriteLine("Passed " + assertions + " mapping manager assertions.");
            return 0;
        }
        catch (Exception exception)
        {
            Console.Error.WriteLine(exception);
            return 1;
        }
    }

    private static void Assert(bool condition, string message)
    {
        assertions++;
        if (!condition)
        {
            throw new InvalidOperationException(message);
        }
    }

    private static StringAttributeMetadata Text(string name, int length = 100)
    {
        var metadata = new StringAttributeMetadata
        {
            LogicalName = name,
            MaxLength = length,
            Format = Microsoft.Xrm.Sdk.Metadata.StringFormat.Text,
            MetadataId = Guid.NewGuid(),
            DisplayName = new Microsoft.Xrm.Sdk.Label(name, 1033),
        };
        Set(metadata, "IsValidForCreate", true);
        return metadata;
    }

    private static void Set(object instance, string property, object value)
    {
        instance.GetType().GetProperty(property).SetValue(instance, value, null);
    }

    private static EntityMetadata Table(string name, params AttributeMetadata[] attributes)
    {
        var table = new EntityMetadata { LogicalName = name, MetadataId = Guid.NewGuid() };
        Set(table, "Attributes", attributes);
        Set(table, "OneToManyRelationships", Array.Empty<OneToManyRelationshipMetadata>());
        return table;
    }

    private static void EditingAndRefresh()
    {
        var fake = new FakeDataverse();
        fake.Add("a", "x");
        fake.Add("b", "y");
        var service = new MappingService(fake);
        var workspace = service.Load(fake.Map);
        var row = workspace.Rows.Single(item => item.Source == "a");
        using (var dialog = new MappingEditForm(workspace, row))
        {
            Assert(
                !((Button)Field(dialog, "apply")).Enabled,
                "Unchanged mapping must not allow no-op edits"
            );
            Assert(dialog.Icon != null, "Mapping editor must use the application icon");
            var origin = (ComboBox)Field(dialog, "source");
            var destination = (ComboBox)Field(dialog, "target");
            origin.SelectedItem = origin.Items.Cast<ColumnItem>().Single(item => item.Name == "c");
            destination.SelectedItem = destination
                .Items.Cast<ColumnItem>()
                .Single(item => item.Name == "y");
            Assert(
                !((Button)Field(dialog, "apply")).Enabled,
                "Editor must reject a target occupied by another mapping"
            );
            destination.SelectedItem = destination
                .Items.Cast<ColumnItem>()
                .Single(item => item.Name == "x");
            Assert(
                ((Button)Field(dialog, "apply")).Enabled,
                "Editor must allow compatible replacement columns"
            );
            Assert(
                !workspace.HasPending,
                "Choosing editor values must not stage changes before Apply"
            );
            Capture(dialog, "mapping-editor-preview.png");
            InvokeClick(dialog, "ApplyClick");
            Assert(
                row.Source == "c" && row.Pending && fake.Records.Count == 2,
                "Applying editor must stage changes without writing to Dataverse"
            );
        }
        workspace.NeedsPublication = true;
        workspace.RefreshMappings(service.GetRows(fake.Map.Id));
        Assert(
            ReferenceEquals(row, workspace.Rows.Single(item => item.Target == "x"))
                && row.Source == "c",
            "Mapping refresh must preserve local changes when the server baseline is unchanged"
        );
        fake.Add("c", "z");
        var conflict = false;
        try
        {
            workspace.RefreshMappings(service.GetRows(fake.Map.Id));
        }
        catch (InvalidOperationException)
        {
            conflict = true;
        }
        Assert(
            conflict && row.Source == "c" && workspace.Rows.Count == 2,
            "Concurrent mapping refresh must preserve pending work and its baseline"
        );
        row.Undo();
        workspace.RefreshMappings(service.GetRows(fake.Map.Id));
        Assert(
            workspace.Rows.Count == 3
                && workspace.Snapshot.Count == 3
                && workspace.NeedsPublication,
            "Clean mapping refresh must load new server mappings without clearing publication state"
        );
        Assert(
            !ReferenceEquals(workspace.Rows[0], workspace.Snapshot[0]),
            "Refreshed snapshot must remain independent of editable rows"
        );
        Assert(
            service.GetTableColumns(fake.Map.Source, true).LogicalName == fake.Map.Source,
            "Source refresh must retrieve the selected source metadata"
        );
        using (var cancelled = new MappingEditForm(workspace, workspace.Rows[0]))
        {
            var origin = (ComboBox)Field(cancelled, "source");
            origin.SelectedItem = origin.Items.Cast<ColumnItem>().Single(item => item.Name == "c");
        }
        Assert(
            !workspace.HasPending,
            "Closing the editor without Apply must preserve existing mappings"
        );
        using (var control = new MappingControl())
        {
            control.Size = new Size(900, 600);
            control.CreateControl();
            typeof(MappingControl)
                .GetField("workspace", BindingFlags.NonPublic | BindingFlags.Instance)
                .SetValue(control, workspace);
            Invoke(control, "RefreshLists");
            Invoke(control, "ShowMappingStep");
            var footer = (FlowLayoutPanel)Field(control, "footerActions");
            Assert(
                ((Button)Field(control, "addMapping")).Parent == footer
                    && ((Button)Field(control, "clearChecks")).Parent == footer,
                "Creation and selection clearing must sit beside review/save"
            );
            Assert(
                ((Button)Field(control, "clearChecks")).Text == "Limpar seleções",
                "Clearing selections must use the plural label"
            );
            Assert(
                ((Button)Field(control, "refreshSource")).Parent
                    != ((Button)Field(control, "refreshTarget")).Parent,
                "Column refresh actions must belong to their own lists"
            );
            Capture(control, "mapping-manager-compact-preview.png");
        }
    }

    private static void InvokeClick(object instance, string name)
    {
        instance
            .GetType()
            .GetMethod(name, BindingFlags.NonPublic | BindingFlags.Instance)
            .Invoke(instance, new object[] { null, EventArgs.Empty });
    }

    private static void Rules()
    {
        Assert(
            MappingRules.Incompatibility(Text("a"), Text("b")) == "",
            "Matching strings must be compatible"
        );
        Assert(
            MappingRules.Incompatibility(Text("a", 200), Text("b", 100)).Contains("Tamanho"),
            "Short destination must be rejected"
        );
        var url = Text("url");
        url.Format = Microsoft.Xrm.Sdk.Metadata.StringFormat.Url;
        Assert(
            MappingRules.Incompatibility(Text("a"), url).Contains("Formatos"),
            "Different text format must be rejected"
        );
        Assert(
            MappingRules
                .Incompatibility(Text("a"), new IntegerAttributeMetadata())
                .Contains("Tipos"),
            "Different types must be rejected"
        );
        var readOnly = Text("readonly");
        Set(readOnly, "IsValidForCreate", false);
        Assert(
            MappingRules.Incompatibility(Text("a"), readOnly).Contains("entrada"),
            "Read-only destination must be rejected"
        );
        Assert(
            MappingRules.Incompatibility(null, Text("b")).Length > 0,
            "Missing selection must be rejected"
        );
        var fake = new FakeDataverse();
        var workspace = new MappingService(fake).Load(fake.Map);
        workspace.Rows.Add(new MappingRow { Source = "a", Target = "x" });
        workspace.Rows.Add(new MappingRow { Source = "b", Target = "x" });
        Assert(
            MappingRules.Validate(workspace).Contains("mais de um"),
            "Duplicate destinations must be rejected"
        );
        workspace.Rows.RemoveAt(1);
        workspace.Rows[0].Protected = true;
        Assert(
            MappingRules.Validate(workspace).Contains("sistema"),
            "Protected edits must be rejected"
        );
        workspace.Rows[0].Protected = false;
        workspace.Rows.Add(new MappingRow { Source = "a", Target = "y" });
        Assert(
            MappingRules.Validate(workspace) == "",
            "One source may map to multiple destinations"
        );
    }

    private static void SaveAndRetry()
    {
        var fake = new FakeDataverse();
        var service = new MappingService(fake);
        var workspace = service.Load(fake.Map);
        workspace.Rows.Add(new MappingRow { Source = "a", Target = "x" });
        workspace.Rows.Add(new MappingRow { Source = "b", Target = "y" });
        fake.FailTarget = "y";
        var results = service.Save(workspace);
        Assert(
            results.Count(result => result.Success) == 1,
            "Independent successful change must survive another failure"
        );
        Assert(results.Count(result => !result.Success) == 1, "Failed change must be reported");
        Assert(
            fake.Records.Count == 1 && workspace.Snapshot.Count == 1,
            "Snapshot must reflect committed operations only"
        );
        Assert(workspace.Rows.Count(row => row.Pending) == 1, "Failed change must remain pending");
        Assert(workspace.NeedsPublication, "Saving requires publication");
        fake.FailPublish = true;
        try
        {
            service.Publish(workspace);
            throw new Exception("Publication should have failed");
        }
        catch (InvalidOperationException)
        {
            Assert(workspace.NeedsPublication, "Publication failure must preserve retry state");
        }
        fake.FailPublish = false;
        service.Publish(workspace);
        Assert(!workspace.NeedsPublication, "Publication success must clear retry state");
        fake.FailTarget = null;
        results = service.Save(workspace);
        Assert(
            results.Count == 1 && results[0].Success,
            "Retry must save only the failed operation"
        );
        Assert(
            !workspace.HasPending && fake.Records.Count == 2,
            "Retry must not duplicate already committed mappings"
        );
        var existing = workspace.Rows.First();
        existing.Source = "c";
        results = service.Save(workspace);
        Assert(
            results.All(result => result.Success) && fake.Records.Count == 2,
            "Edit must replace the mapping atomically"
        );
        existing.Removed = true;
        results = service.Save(workspace);
        Assert(
            results.Single().Success && fake.Records.Count == 1,
            "Removal must commit and update the snapshot"
        );
    }

    private static void SwapAndRollback()
    {
        var fake = new FakeDataverse();
        fake.Add("a", "x");
        fake.Add("b", "y");
        var service = new MappingService(fake);
        var workspace = service.Load(fake.Map);
        workspace.Rows[0].Target = "y";
        workspace.Rows[1].Target = "x";
        Assert(
            MappingService.GroupChanges(workspace.Rows).Count == 1,
            "Destination swap must be grouped atomically"
        );
        fake.FailTarget = "y";
        var results = service.Save(workspace);
        Assert(results.All(result => !result.Success), "Entire failed group must report failure");
        Assert(
            fake.Records.Any(record =>
                record.GetAttributeValue<string>("sourceattributename") == "a"
                && record.GetAttributeValue<string>("targetattributename") == "x"
            ),
            "Failed edit must preserve original mappings"
        );
        Assert(workspace.Rows.All(row => row.Pending), "Rolled-back changes must remain pending");
        fake.FailTarget = null;
        results = service.Save(workspace);
        Assert(
            results.All(result => result.Success) && !workspace.HasPending,
            "Swap retry must succeed atomically"
        );
        Assert(
            fake.Records.Select(record => record.GetAttributeValue<string>("targetattributename"))
                .Distinct()
                .Count() == 2,
            "Swap must retain unique destinations"
        );
    }

    private static void Conflict()
    {
        var fake = new FakeDataverse();
        var service = new MappingService(fake);
        var workspace = service.Load(fake.Map);
        workspace.Rows.Add(new MappingRow { Source = "a", Target = "x" });
        fake.Add("b", "y");
        try
        {
            service.Save(workspace);
            throw new Exception("Concurrent change should have blocked save");
        }
        catch (InvalidOperationException exception)
        {
            Assert(
                exception.Message.Contains("mudaram"),
                "Concurrent change must give reload instructions"
            );
            Assert(fake.TransactionCount == 0, "Conflict must be detected before any writes");
        }
    }

    private static void UncertainSave()
    {
        var fake = new FakeDataverse();
        var service = new MappingService(fake);
        var workspace = service.Load(fake.Map);
        workspace.Rows.Add(new MappingRow { Source = "a", Target = "x" });
        workspace.Rows.Add(new MappingRow { Source = "b", Target = "y" });
        fake.LoseResponse = true;
        var results = service.Save(workspace);
        Assert(workspace.RequiresReload, "Lost response must require reload");
        Assert(fake.TransactionCount == 1, "Uncertain result must stop later transactions");
        Assert(
            results.Count == 2 && results.All(result => !result.Success),
            "Unconfirmed and skipped operations must have explicit results"
        );
        Assert(fake.Records.Count == 1, "Server may have committed despite lost response");
        try
        {
            service.Save(workspace);
            throw new Exception("Uncertain result should prevent retry");
        }
        catch (InvalidOperationException exception)
        {
            Assert(exception.Message.Contains("incerto"), "Uncertain save must not silently retry");
        }
    }

    private static void DesignerLoading()
    {
        foreach (
            var type in new[]
            {
                typeof(MappingControl),
                typeof(MappingEditForm),
                typeof(ReviewForm),
                typeof(SolutionForm),
            }
        )
        {
            using (var surface = new DesignSurface(type))
            {
                Assert(
                    surface.IsLoaded,
                    type.Name
                        + " must load in the WinForms designer infrastructure: "
                        + string.Join("; ", surface.LoadErrors.Cast<object>())
                );
                Assert(
                    surface.LoadErrors.Count == 0,
                    type.Name + " must have no designer load errors"
                );
            }
        }
    }

    private static void SolutionComponents()
    {
        var fake = new FakeDataverse();
        var relation = new OneToManyRelationshipMetadata
        {
            MetadataId = Guid.NewGuid(),
            SchemaName = "source_target",
            ReferencedEntity = "source",
            ReferencingEntity = "target",
        };
        Set(fake.Source, "OneToManyRelationships", new[] { relation });
        var service = new MappingService(fake);
        var workspace = service.Load(fake.Map);
        workspace.Rows.Add(new MappingRow { Source = "a", Target = "x" });
        service.Save(workspace);
        var components = service.GetComponentsForMappings(workspace, workspace.ExportExpectations);
        Assert(components.Count(item => item.Type == 1) == 2, "Both table shells must be included");
        Assert(
            components.Any(item => item.Type == 10 && item.Id == relation.MetadataId),
            "Related relationship must be offered"
        );
        Assert(
            components.All(item => item.Type != 46)
                && components.Any(item => item.ExportExpectation != null),
            "Mapping transport must be verified through export, without adding EntityMap directly"
        );
        Assert(
            components.Count(item => item.Type == 2) == 2,
            "Changed source and target columns must be included"
        );
        var results = service.AddToSolution("test_solution", components);
        Assert(
            results.All(result => result.Success),
            "Solution inclusion must report each component"
        );
        Assert(
            fake.SolutionRequests.Where(request => (int)request["ComponentType"] == 1)
                .All(request => (bool)request["DoNotIncludeSubcomponents"]),
            "Table shells must not include unrelated subcomponents"
        );
        Assert(
            fake.SolutionRequests.Where(request => (int)request["ComponentType"] != 1)
                .All(request => !request.Parameters.Contains("DoNotIncludeSubcomponents")),
            "Non-table components must not receive unsupported shell flag"
        );
        Assert(
            fake.SolutionRequests.All(request => !(bool)request["AddRequiredComponents"]),
            "Dependency expansion must remain scoped"
        );
        Assert(
            fake.SolutionRequests.All(request =>
                (int)request["ComponentType"] != 46 && (int)request["ComponentType"] != 47
            ),
            "Unsupported map subcomponents must never be sent to AddSolutionComponent"
        );
        Assert(
            results.Any(result => result.State == "Verificado no pacote" && result.Success),
            "Mapping success must require its exact pair in exported XML"
        );
        fake.OmitExportMappings = true;
        results = service.AddToSolution("test_solution", components);
        Assert(
            results.Any(result => result.State == "Não confirmado" && !result.Success),
            "Successful component addition must not hide a missing exported mapping"
        );
        fake.OmitExportMappings = false;
        fake.FailExport = true;
        results = service.AddToSolution("test_solution", components);
        Assert(
            results.Any(result =>
                result.State == "Não confirmado" && result.Detail.Contains("export")
            ),
            "Export permission failure must leave mapping verification unconfirmed"
        );
        fake.FailExport = false;
        var loaded = service.Load(fake.Map);
        var existingComponents = service.GetComponentsForMappings(
            loaded,
            service.GetSolutionChoices(loaded).Select(choice => choice.Expectation)
        );
        Assert(
            existingComponents.Any(item => item.ExportExpectation != null)
                && existingComponents.Count(item => item.Type == 2) == 2,
            "A previously saved checked mapping must be addable without repeating its write"
        );
        workspace.Rows[0].Removed = true;
        service.Save(workspace);
        results = service.AddToSolution(
            "test_solution",
            service.GetComponentsForMappings(workspace, workspace.ExportExpectations)
        );
        Assert(
            results.Any(result =>
                result.State == "Atenção" && !result.Success && result.Detail.Contains("upgrade")
            ),
            "Absence from export must not falsely confirm deletion in another environment"
        );
    }

    private static void ExportVerification()
    {
        var expected = new MappingExportExpectation
        {
            SourceTable = "lead",
            TargetTable = "contact",
            SourceColumn = "address1_county",
            TargetColumn = "address1_county",
            ShouldExist = true,
        };
        var item = new SolutionComponentItem(47, Guid.NewGuid(), "Verify lead/contact", expected);
        var xml =
            "<ImportExportXml><EntityMaps><EntityMap><EntitySource>lead</EntitySource><EntityTarget>contact</EntityTarget><AttributeMaps><AttributeMap><AttributeSource>address1_county</AttributeSource><AttributeTarget>address1_county</AttributeTarget></AttributeMap></AttributeMaps></EntityMap></EntityMaps></ImportExportXml>";
        Assert(
            SolutionMappingExportVerifier
                .Verify(CreateSolutionZip(xml), new[] { item })
                .Single()
                .Success,
            "Exact exported mapping must be recognized"
        );
        Assert(
            !SolutionMappingExportVerifier
                .Verify(
                    CreateSolutionZip(
                        xml.Replace("<EntityTarget>contact", "<EntityTarget>account")
                    ),
                    new[] { item }
                )
                .Single()
                .Success,
            "Same column names on another table pair must not count as transport proof"
        );
        Assert(
            !SolutionMappingExportVerifier
                .Verify(
                    CreateSolutionZip(
                        xml.Replace(
                            "<AttributeTarget>address1_county",
                            "<AttributeTarget>address2_county"
                        )
                    ),
                    new[] { item }
                )
                .Single()
                .Success,
            "Different target column must not count as transport proof"
        );
        Assert(
            SolutionMappingExportVerifier
                .Verify(
                    CreateSolutionZip(
                        xml.Replace("<ImportExportXml>", "<ImportExportXml xmlns='urn:test'>")
                            .Replace(">lead<", "> LEAD <")
                    ),
                    new[] { item }
                )
                .Single()
                .Success,
            "Namespace and logical-name casing must not prevent verification"
        );
        var fake = new FakeDataverse();
        fake.Add("a", "x");
        var service = new MappingService(fake);
        var workspace = service.Load(fake.Map);
        workspace.Rows[0].Removed = true;
        workspace.Rows.Add(new MappingRow { Source = "a", Target = "x" });
        Assert(
            service.Save(workspace).All(result => result.Success),
            "Replacement of a removed identical pair must commit atomically"
        );
        Assert(
            workspace.ExportExpectations.Count == 1 && workspace.ExportExpectations[0].ShouldExist,
            "Export expectation must reflect final committed snapshot, not transaction iteration order"
        );
    }

    private static void NativeExportOmission()
    {
        var fake = new FakeDataverse { OmitSystemMappings = true };
        fake.Add("a", "x");
        fake.Records[0]["issystem"] = true;
        fake.Records[0]["ismanaged"] = true;
        fake.Add("b", "y");
        var service = new MappingService(fake);
        var workspace = service.Load(fake.Map);
        Assert(
            workspace.Rows[0].IsSystem
                && workspace.Rows[0].IsManaged
                && workspace.Rows[0].Protected,
            "System and managed flags must be retained separately while keeping edit protection"
        );
        Assert(
            workspace.Snapshot[0].IsSystem && workspace.Snapshot[0].IsManaged,
            "Snapshot must retain mapping origin flags"
        );
        var choices = service.GetSolutionChoices(workspace);
        Assert(
            choices[0].Expectation.IsSystem && choices[0].Status.Contains("sistema"),
            "Solution selection must identify native mappings by flags"
        );
        var components = service.GetComponentsForMappings(
            workspace,
            choices.Select(choice => choice.Expectation)
        );
        var results = service.AddToSolution("test_solution", components);
        var native = results.Single(result => result.State == "Nativo não exportado");
        Assert(
            !native.Success
                && native.IsWarning
                && native.Detail.Contains("a existência lá não foi verificada"),
            "Omitted native mapping must be a warning without claiming target availability"
        );
        Assert(
            results.Any(result => result.State == "Verificado no pacote" && result.Success),
            "Custom mapping in same export must remain verified"
        );
        using (var form = new ReviewForm("Resultado da solution", results))
        {
            form.CreateControl();
            var status = ((System.Windows.Forms.Label)Field(form, "executionStatus")).Text;
            Assert(
                status.Contains("1 aviso(s)") && status.Contains("0 falha(s)"),
                "Native export warning must not be counted as a failed component addition"
            );
            Capture(form, "solution-native-warning-preview.png");
        }
        fake.OmitSystemMappings = false;
        results = service.AddToSolution("test_solution", components);
        Assert(
            results
                .Where(result => result.Operation.StartsWith("Verificar"))
                .All(result => result.Success),
            "A native mapping actually present in XML must still be verified"
        );
        fake.Records[0]["issystem"] = false;
        fake.OmitExportMappings = true;
        workspace = service.Load(fake.Map);
        components = service.GetComponentsForMappings(
            workspace,
            service.GetSolutionChoices(workspace).Select(choice => choice.Expectation)
        );
        results = service.AddToSolution("test_solution", components);
        Assert(
            results.Any(result => result.State == "Gerenciado não exportado" && result.IsWarning),
            "Managed-only mappings must be distinct from system mappings"
        );
        Assert(
            results.Any(result => result.State == "Não confirmado" && result.IsWarning),
            "An omitted ordinary mapping must keep generic unconfirmed status"
        );
    }

    private static void SetChoice(DataGridView grid, string source, string target, bool selected)
    {
        var row = grid
            .Rows.Cast<DataGridViewRow>()
            .Single(item =>
                ((SolutionMappingChoice)item.Tag).Source == source
                && ((SolutionMappingChoice)item.Tag).Target == target
            );
        row.Cells[0].Value = selected;
        Application.DoEvents();
    }

    private static void SolutionSelection()
    {
        var fake = new FakeDataverse();
        fake.Add("a", "x");
        fake.Add("b", "y");
        var service = new MappingService(fake);
        var workspace = service.Load(fake.Map);
        workspace.Rows[0].Source = "c";
        workspace.Rows.Add(new MappingRow { Source = "c", Target = "z" });
        Assert(
            service.Save(workspace).All(result => result.Success),
            "Selection scenario must commit one edit and one addition"
        );
        var choices = service.GetSolutionChoices(workspace);
        Assert(
            choices.Count(choice => !choice.IsRemoval) == 3,
            "Solution modal must offer all loaded active mappings"
        );
        Assert(
            choices.Count(choice => choice.SelectedByDefault) == 2,
            "Only successful additions and edits in this session must be prechecked"
        );
        Assert(
            choices.Single(choice => choice.Source == "b").Available
                && !choices.Single(choice => choice.Source == "b").SelectedByDefault,
            "Existing mapping from another session must remain available but unchecked"
        );
        Assert(
            choices.Count(choice => choice.IsRemoval) == 1
                && !choices.Single(choice => choice.IsRemoval).SelectedByDefault,
            "Saved old side of an edit must be separate and unchecked"
        );
        var reloaded = service.GetSolutionChoices(service.Load(fake.Map));
        Assert(
            reloaded.All(choice => choice.Available && !choice.SelectedByDefault),
            "Reloaded mappings must not be mistaken for changes made in this session"
        );
        using (
            var form = new SolutionForm(
                new[]
                {
                    new PickerItem { Name = "test", Label = "Test" },
                },
                choices,
                selected => service.GetComponentsForMappings(workspace, selected)
            )
        )
        {
            form.CreateControl();
            var mappings = (DataGridView)Field(form, "mappingsGrid");
            var removals = (DataGridView)Field(form, "removalsGrid");
            Assert(
                mappings.Rows.Count == 3 && removals.Rows.Count == 1,
                "Transport and removal sections must be separate"
            );
            Assert(
                form.SelectedComponents.Count(item => item.ExportExpectation != null) == 2,
                "Only default checked mappings should be verified initially"
            );
            SetChoice(mappings, "b", "y", true);
            Assert(
                form.SelectedComponents.Count(item => item.ExportExpectation != null) == 3,
                "User must be able to include an existing mapping"
            );
            var sourceB = workspace
                .Source.Attributes.Single(column => column.LogicalName == "b")
                .MetadataId.Value;
            var targetY = workspace
                .Target.Attributes.Single(column => column.LogicalName == "y")
                .MetadataId.Value;
            Assert(
                form.SelectedComponents.Any(item => item.Type == 2 && item.Id == sourceB)
                    && form.SelectedComponents.Any(item => item.Type == 2 && item.Id == targetY),
                "Checking an existing mapping must include its two columns"
            );
            SetChoice(mappings, "c", "x", false);
            var targetX = workspace
                .Target.Attributes.Single(column => column.LogicalName == "x")
                .MetadataId.Value;
            Assert(
                form.SelectedComponents.All(item => item.Type != 2 || item.Id != targetX),
                "Unchecking a mapping must not leave its unrelated column selected"
            );
            ((TextBox)Field(form, "mappingSearch")).Text = "b";
            Assert(
                mappings.Rows.Count == 1
                    && form.SelectedComponents.Count(item => item.ExportExpectation != null) == 2,
                "Search must preserve checked choices that are hidden"
            );
            ((TextBox)Field(form, "mappingSearch")).Clear();
            Capture(form, "solution-mappings-preview.png");
            ((TabControl)Field(form, "sections")).SelectedIndex = 1;
            Capture(form, "solution-components-preview.png");
            ((TabControl)Field(form, "sections")).SelectedIndex = 2;
            Capture(form, "solution-removals-preview.png");
            SetChoice(removals, "a", "x", true);
            Assert(
                form.SelectedComponents.Any(item => item.ExportExpectation?.ShouldExist == false),
                "Absence checks must be explicitly selectable in the removals section"
            );
            typeof(SolutionForm)
                .GetMethod("ClearSelectedClick", BindingFlags.Instance | BindingFlags.NonPublic)
                .Invoke(form, new object[] { null, EventArgs.Empty });
            Assert(
                form.SelectedComponents.Count == 0,
                "Clearing choices must clear transport, removal checks and derived dependencies"
            );
            Assert(
                fake.SolutionRequests.Count == 0,
                "Reviewing choices must not write anything to Dataverse"
            );
        }
        workspace.Rows.Add(new MappingRow { Source = "a", Target = "y" });
        workspace.Rows.Single(row => row.Source == "c" && row.Target == "z").Source = "a";
        workspace.Rows.Single(row => row.Source == "b").Removed = true;
        var pendingChoices = service.GetSolutionChoices(workspace);
        Assert(
            pendingChoices.Count(choice => !choice.Available) == 3,
            "Unsaved additions, edits and removals must be unavailable"
        );
        Assert(
            pendingChoices.Count(choice => choice.SelectedByDefault) == 1,
            "A new local edit must clear preselection of its previously saved version"
        );
        using (
            var form = new SolutionForm(
                Array.Empty<PickerItem>(),
                pendingChoices,
                selected => service.GetComponentsForMappings(workspace, selected)
            )
        )
        {
            form.CreateControl();
            var grid = (DataGridView)Field(form, "mappingsGrid");
            Assert(
                grid.Rows.Cast<DataGridViewRow>()
                    .Where(row => !((SolutionMappingChoice)row.Tag).Available)
                    .All(row => row.ReadOnly),
                "Unsaved rows must be visibly read-only"
            );
            SetChoice(grid, "a", "y", true);
            Assert(
                form.SelectedComponents.All(item =>
                    item.ExportExpectation == null || item.ExportExpectation.SourceColumn != "a"
                ),
                "Unsaved mappings must not enter the transport plan even through a programmatic checkbox change"
            );
            typeof(SolutionForm)
                .GetMethod("SelectAllClick", BindingFlags.Instance | BindingFlags.NonPublic)
                .Invoke(form, new object[] { null, EventArgs.Empty });
            Assert(
                form.SelectedComponents.Count(item => item.ExportExpectation != null) == 1,
                "Select all must skip unsaved mappings and optional removal checks"
            );
        }
        var failedFake = new FakeDataverse { FailTarget = "y" };
        failedFake.Add("a", "x");
        var failedService = new MappingService(failedFake);
        var failedWorkspace = failedService.Load(failedFake.Map);
        failedWorkspace.Rows.Add(new MappingRow { Source = "b", Target = "y" });
        failedService.Save(failedWorkspace);
        Assert(
            failedService
                .GetSolutionChoices(failedWorkspace)
                .All(choice => !choice.SelectedByDefault),
            "A failed save must never be prechecked as a newly saved mapping"
        );
    }

    private static object Field(object instance, string name)
    {
        return instance
            .GetType()
            .GetField(name, BindingFlags.NonPublic | BindingFlags.Instance)
            .GetValue(instance);
    }

    private static void Invoke(object instance, string name)
    {
        instance
            .GetType()
            .GetMethod(name, BindingFlags.NonPublic | BindingFlags.Instance)
            .Invoke(instance, null);
    }

    private static void ClickCheck(MappingControl control, DataGridView grid, string identity)
    {
        var selectedRow = grid
            .Rows.Cast<DataGridViewRow>()
            .Single(row =>
                row.Tag is ColumnItem column
                    ? column.Name == identity
                    : ((MappingRow)row.Tag).Source + ":" + ((MappingRow)row.Tag).Target == identity
            );
        var rowIndex = selectedRow.Index;
        typeof(MappingControl)
            .GetMethod("GridCheckboxClick", BindingFlags.NonPublic | BindingFlags.Instance)
            .Invoke(control, new object[] { grid, new DataGridViewCellEventArgs(0, rowIndex) });
        Application.DoEvents();
    }

    private static void UserInterface()
    {
        var fake = new FakeDataverse();
        fake.Add("a", "x");
        var workspace = new MappingService(fake).Load(fake.Map);
        using (var control = new MappingControl())
        {
            control.Size = new Size(1600, 900);
            control.CreateControl();
            typeof(MappingControl)
                .GetField("workspace", BindingFlags.NonPublic | BindingFlags.Instance)
                .SetValue(control, workspace);
            Invoke(control, "RefreshLists");
            Invoke(control, "ShowMappingStep");
            var source = (DataGridView)Field(control, "sourceGrid");
            var target = (DataGridView)Field(control, "targetGrid");
            var mappings = (DataGridView)Field(control, "mappingGrid");
            Assert(source.Rows.Count == 3 && target.Rows.Count == 3, "Both column lists must load");
            Assert(
                new[] { source, target, mappings }.All(grid =>
                    grid.Columns[0] is DataGridViewCheckBoxColumn
                ),
                "All three lists must have checkbox columns"
            );
            source.CurrentCell = source.Rows[0].Cells[1];
            mappings.CurrentCell = mappings.Rows[0].Cells[1];
            Application.DoEvents();
            Assert(
                Field(control, "checkedSource") == null && Field(control, "checkedMapping") == null,
                "Focusing a row must not select a column or mapping"
            );
            Assert(
                target
                    .Rows.Cast<DataGridViewRow>()
                    .All(row => Convert.ToString(row.Cells["Reason"].Value) == ""),
                "Row focus must not change compatibility emphasis"
            );
            Assert(
                !((Button)Field(control, "editMapping")).Enabled,
                "Row focus must not enable mapping editing"
            );
            ClickCheck(control, source, "a");
            Assert(
                target
                    .Rows.Cast<DataGridViewRow>()
                    .Any(row => Convert.ToString(row.Cells["Reason"].Value) == "Já mapeado"),
                "Checking source must emphasize mapped target"
            );
            ClickCheck(control, mappings, "a:x");
            Assert(
                (string)Field(control, "checkedSource") == "a"
                    && (string)Field(control, "checkedTarget") == "x",
                "Checking mapping must check its two columns"
            );
            Assert(
                Convert.ToBoolean(source.Rows[0].Cells[0].Value)
                    && Convert.ToBoolean(target.Rows[0].Cells[0].Value),
                "Linked columns must show checked boxes"
            );
            ClickCheck(control, target, "y");
            Assert(((ColumnItem)target.Rows[0].Tag).Name == "y", "Checked target must move to top");
            Assert(
                source
                    .Rows.Cast<DataGridViewRow>()
                    .Any(row => Convert.ToString(row.Cells["Reason"].Value).Contains("Compatível")),
                "Checking target must show compatible sources"
            );
            Assert(
                target
                    .Rows.Cast<DataGridViewRow>()
                    .Count(row => Convert.ToBoolean(row.Cells[0].Value)) == 1,
                "Only one checkbox per list may be checked"
            );
            ClickCheck(control, source, "c");
            Assert(((ColumnItem)source.Rows[0].Tag).Name == "c", "Checked column must move to top");
            Assert(
                ((ColumnItem)source.Rows[1].Tag).Name == "a"
                    && ((ColumnItem)source.Rows[2].Tag).Name == "b",
                "Unchecked columns must remain sorted by logical name"
            );
            Assert(
                source.Columns[1].Name == "Name"
                    && source.Columns[2].Name == "Label"
                    && target.Columns[1].Name == "Name"
                    && target.Columns[2].Name == "Label",
                "Logical name must precede display name in both column lists"
            );
            Assert(
                Convert.ToString(source.Rows[0].Cells["Name"].Value) == "c",
                "Logical name data must match its header"
            );
            using (var dialog = new MappingEditForm(workspace, workspace.Rows[0]))
            {
                var editSource = (ComboBox)Field(dialog, "source");
                editSource.SelectedItem = editSource
                    .Items.Cast<ColumnItem>()
                    .Single(item => item.Name == "c");
                ((Button)Field(dialog, "apply")).PerformClick();
                typeof(MappingEditForm)
                    .GetMethod("ApplyClick", BindingFlags.NonPublic | BindingFlags.Instance)
                    .Invoke(dialog, new object[] { null, EventArgs.Empty });
            }
            Assert(
                workspace.Rows[0].Source == "c" && workspace.Rows[0].Change == "Alterado",
                "Checked mapping must receive staged edit"
            );
            typeof(MappingControl)
                .GetMethod("UndoMappingClick", BindingFlags.NonPublic | BindingFlags.Instance)
                .Invoke(control, new object[] { null, EventArgs.Empty });
            Assert(
                workspace.Rows[0].Source == "a" && !workspace.HasPending,
                "Undo must restore original pair"
            );
            ClickCheck(control, source, "b");
            typeof(MappingControl)
                .GetMethod("AddMappingClick", BindingFlags.NonPublic | BindingFlags.Instance)
                .Invoke(control, new object[] { null, EventArgs.Empty });
            Assert(
                workspace.Rows.Count == 2 && fake.Records.Count == 1,
                "Checkbox-based creation must remain local until save"
            );
            Assert(
                ((Button)Field(control, "reviewStep")).Enabled,
                "Pending changes must enable review navigation"
            );
            typeof(MappingControl)
                .GetMethod("TablesStepClick", BindingFlags.NonPublic | BindingFlags.Instance)
                .Invoke(control, new object[] { null, EventArgs.Empty });
            Assert(
                (int)Field(control, "workflowStep") == 1
                    && ReferenceEquals(Field(control, "workspace"), workspace)
                    && workspace.HasPending,
                "Returning to tables must preserve staged work"
            );
            typeof(MappingControl)
                .GetMethod("MappingStepClick", BindingFlags.NonPublic | BindingFlags.Instance)
                .Invoke(control, new object[] { null, EventArgs.Empty });
            Assert(
                (int)Field(control, "workflowStep") == 2
                    && ((Button)Field(control, "saveChanges")).Enabled,
                "Returning to mapping must restore review access"
            );
            Assert(
                source.RowTemplate.Height == 23 && target.RowTemplate.Height == 23,
                "Column lists must retain compact rows"
            );
            Assert(
                control.BackColor == SystemColors.Control,
                "Workspace must use the standard system palette"
            );
            ClickCheck(control, mappings, "b:y");
            Assert(
                ((MappingRow)mappings.Rows[0].Tag).Source == "a"
                    && ((MappingRow)mappings.Rows[1].Tag).Source == "b"
                    && Convert.ToBoolean(mappings.Rows[1].Cells[0].Value),
                "Checking a mapping must preserve logical-name ordering"
            );
            typeof(MappingControl)
                .GetMethod("RemoveMappingClick", BindingFlags.NonPublic | BindingFlags.Instance)
                .Invoke(control, new object[] { null, EventArgs.Empty });
            Assert(
                workspace.Rows.Count == 1 && fake.Records.Count == 1,
                "Removing checked staged addition must cancel it locally"
            );
            var pending = new MappingRow { Source = "b", Target = "y" };
            workspace.Rows.Add(pending);
            Invoke(control, "RefreshLists");
            Invoke(control, "ShowMappingStep");
            Assert(
                ((Button)Field(control, "saveChanges")).Enabled,
                "Pending changes must enable save"
            );
            Capture(control, "mapping-manager-preview.png");
            typeof(MappingControl)
                .GetMethod("ClearChecksClick", BindingFlags.NonPublic | BindingFlags.Instance)
                .Invoke(control, new object[] { null, EventArgs.Empty });
            Assert(
                new[] { source, target, mappings }.All(grid =>
                    grid.Rows.Cast<DataGridViewRow>()
                        .All(row => !Convert.ToBoolean(row.Cells[0].Value))
                ),
                "Clear checks must clear all lists"
            );
            ClickCheck(control, source, "a");
            ClickCheck(control, source, "a");
            Assert(
                Field(control, "checkedSource") == null,
                "Clicking checked box again must deselect it"
            );
            ClickCheck(control, source, "a");
            ((TextBox)Field(control, "sourceSearch")).Text = "c";
            Assert(
                (string)Field(control, "checkedSource") == "a",
                "Filtering must preserve checked identity even when hidden"
            );
            Assert(
                ((System.Windows.Forms.Label)Field(control, "selectionSummary")).Text.Contains("a"),
                "Selection summary must reveal hidden selection"
            );
            workspace.Rows.Remove(pending);
            control.ClosingPlugin(new PluginCloseInfo { Silent = true });
        }
        using (
            var form = new ReviewForm(
                new[]
                {
                    new MappingRow { Source = "a", Target = "x" },
                }
            )
        )
        {
            form.CreateControl();
            Assert(
                ((System.Windows.Forms.Label)Field(form, "summary")).Text.Contains(
                    "Mapeamentos adicionados: 1"
                ),
                "Review must summarize additions"
            );
            Assert(
                ((DataGridView)Field(form, "changesGrid")).Rows.Count == 1,
                "Review must show changes in a grid"
            );
            Assert(form.Icon != null, "Review modal must have application icon");
            Capture(form, "mapping-review-preview.png");
        }
        using (
            var form = new SolutionForm(
                new[]
                {
                    new PickerItem { Name = "test", Label = "Test" },
                },
                new[]
                {
                    new SolutionMappingChoice
                    {
                        Expectation = new MappingExportExpectation
                        {
                            SourceColumn = "a",
                            TargetColumn = "x",
                            ShouldExist = true,
                        },
                        Available = true,
                        SelectedByDefault = true,
                        Status = "Salvo nesta sessão",
                    },
                },
                expected => new List<SolutionComponentItem>
                {
                    new SolutionComponentItem(1, Guid.NewGuid(), "Table"),
                }
            )
        )
        {
            form.CreateControl();
            Assert(
                form.SelectedComponents.Count == 1 && form.Icon != null,
                "Solution modal must show component checklist and app icon"
            );
        }
    }

    private static void MappingCheckSynchronization()
    {
        var fake = new FakeDataverse();
        fake.Add("a", "x");
        fake.Add("b", "y");
        var workspace = new MappingService(fake).Load(fake.Map);
        using (var control = new MappingControl())
        {
            control.CreateControl();
            typeof(MappingControl)
                .GetField("workspace", BindingFlags.NonPublic | BindingFlags.Instance)
                .SetValue(control, workspace);
            Invoke(control, "RefreshLists");
            Invoke(control, "ShowMappingStep");
            var source = (DataGridView)Field(control, "sourceGrid");
            var target = (DataGridView)Field(control, "targetGrid");
            var mappings = (DataGridView)Field(control, "mappingGrid");
            ClickCheck(control, source, "b");
            Assert(
                Field(control, "checkedMapping") == null,
                "One checked column must not select a mapping without a full pair"
            );
            ClickCheck(control, target, "y");
            Assert(
                ReferenceEquals(Field(control, "checkedMapping"), workspace.Rows[1])
                    && Convert.ToBoolean(mappings.Rows[1].Cells[0].Value),
                "Matching pair must automatically check its mapping"
            );
            Assert(
                ((MappingRow)mappings.Rows[0].Tag).Source == "a",
                "Automatic mapping check must not reorder mapping list"
            );
            ClickCheck(control, source, "c");
            Assert(
                Field(control, "checkedMapping") == null,
                "Changing an automatically checked pair to an unmapped pair must clear its old mapping check"
            );
            ClickCheck(control, target, "x");
            ClickCheck(control, source, "a");
            Assert(
                ReferenceEquals(Field(control, "checkedMapping"), workspace.Rows[0]),
                "Automatic selection must work when destination is checked first"
            );
            ClickCheck(control, target, "x");
            Assert(
                Field(control, "checkedMapping") == null,
                "Unchecking part of an automatic pair must clear mapping check"
            );
            ((TextBox)Field(control, "mappingSearch")).Text = "b";
            ClickCheck(control, target, "x");
            Assert(
                mappings.Rows.Count == 2 && Convert.ToBoolean(mappings.Rows[0].Cells[0].Value),
                "Matching mapping hidden by search must be revealed and checked"
            );
            ClickCheck(control, mappings, "a:x");
            ClickCheck(control, mappings, "a:x");
            ClickCheck(control, source, "c");
            Assert(
                ReferenceEquals(Field(control, "checkedMapping"), workspace.Rows[0]),
                "Explicitly checked mapping must remain available for replacement editing"
            );
            workspace.Rows[1].Removed = true;
            ClickCheck(control, source, "b");
            ClickCheck(control, target, "y");
            Assert(
                !ReferenceEquals(Field(control, "checkedMapping"), workspace.Rows[1]),
                "Removed staged mapping must not be auto-selected"
            );
        }
    }

    private static void Capture(Control control, string name)
    {
        var form = control as Form;
        if (form != null)
        {
            form.Opacity = 0;
            form.Show();
            form.PerformLayout();
            Application.DoEvents();
        }
        using (var bitmap = new Bitmap(control.Width, control.Height))
        {
            control.DrawToBitmap(bitmap, new Rectangle(Point.Empty, control.Size));
            bitmap.Save(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, name));
        }
        form?.Hide();
    }

    private static void ReviewExecution()
    {
        var first = new MappingRow { Source = "a", Target = "x" };
        var second = new MappingRow { Source = "b", Target = "y" };
        var completion = new System.Threading.Tasks.TaskCompletionSource<List<OperationResult>>();
        using (
            var form = new ReviewForm(
                new[] { first, second },
                progress =>
                {
                    progress.Report(
                        new MappingProgress
                        {
                            Mapping = first,
                            State = "Em execução",
                            Detail = "Saving first",
                        }
                    );
                    progress.Report(
                        new MappingProgress
                        {
                            Mapping = second,
                            State = "Em execução",
                            Detail = "Saving second",
                        }
                    );
                    return completion.Task;
                }
            )
        )
        {
            form.CreateControl();
            typeof(ReviewForm)
                .GetMethod("ConfirmClick", BindingFlags.NonPublic | BindingFlags.Instance)
                .Invoke(form, new object[] { null, EventArgs.Empty });
            Application.DoEvents();
            var grid = (DataGridView)Field(form, "changesGrid");
            Assert(
                form.DialogResult == DialogResult.None && !((Button)Field(form, "confirm")).Enabled,
                "Confirming must keep modal open and disable repeated saves"
            );
            Assert(
                grid.Rows.Cast<DataGridViewRow>()
                    .All(row => Convert.ToString(row.Cells["State"].Value) == "Em execução"),
                "Every executing mapping must have running state"
            );
            Assert(
                ((Timer)Field(form, "animationTimer")).Enabled,
                "Progress bars must animate while running"
            );
            var close = new FormClosingEventArgs(CloseReason.UserClosing, false);
            typeof(ReviewForm)
                .GetMethod("ReviewFormClosing", BindingFlags.NonPublic | BindingFlags.Instance)
                .Invoke(form, new object[] { form, close });
            Assert(close.Cancel, "Modal must block closing during writes");
            Capture(form, "mapping-progress-preview.png");
            completion.SetResult(
                new List<OperationResult>
                {
                    new OperationResult
                    {
                        Mapping = first,
                        State = "Salvo",
                        Success = true,
                        Detail = "Saved first",
                    },
                    new OperationResult
                    {
                        Mapping = second,
                        State = "Falha",
                        Detail = "Permission denied",
                    },
                }
            );
            var deadline = DateTime.UtcNow.AddSeconds(5);
            while (!form.HasExecuted && DateTime.UtcNow < deadline)
            {
                Application.DoEvents();
                System.Threading.Thread.Sleep(10);
            }
            Assert(
                form.HasExecuted && !((Timer)Field(form, "animationTimer")).Enabled,
                "Completion must stop animation"
            );
            Assert(
                Convert.ToString(grid.Rows[0].Cells["State"].Value) == "Salvo",
                "Successful row must show saved state"
            );
            Assert(
                Convert.ToString(grid.Rows[1].Cells["Detail"].Value) == "Permission denied",
                "Failed row must preserve reason"
            );
            Assert(
                ((Button)Field(form, "confirm")).Text == "Fechar",
                "Completed modal must offer close instead of saving twice"
            );
            Capture(form, "mapping-results-preview.png");
        }
    }

    private static void ReviewSolutionAction()
    {
        foreach (
            var scenario in new[] { "published", "publication-failed", "save-failed", "partial" }
        )
        {
            var mapping = new MappingRow { Source = "a", Target = "x" };
            var mappingSaved = scenario != "save-failed";
            var published = scenario != "publication-failed";
            var results = new List<OperationResult>
            {
                new OperationResult
                {
                    Mapping = mapping,
                    Success = mappingSaved,
                    State = mappingSaved ? "Salvo" : "Falha",
                },
                new OperationResult
                {
                    Operation = "Publicação",
                    Success = published,
                    State = published ? "Publicado" : "Falha",
                },
            };
            if (scenario == "partial")
            {
                results.Add(
                    new OperationResult
                    {
                        Mapping = new MappingRow { Source = "b", Target = "y" },
                        State = "Falha",
                    }
                );
            }
            var solutionCalls = 0;
            IWin32Window solutionOwner = null;
            using (
                var form = new ReviewForm(
                    new[] { mapping },
                    progress => System.Threading.Tasks.Task.FromResult(results),
                    owner =>
                    {
                        solutionOwner = owner;
                        solutionCalls++;
                        return System.Threading.Tasks.Task.CompletedTask;
                    }
                )
            )
            {
                form.CreateControl();
                var button = (Button)Field(form, "addToSolution");
                Assert(!button.Enabled, "Add-to-solution must start disabled before save");
                typeof(ReviewForm)
                    .GetMethod("ConfirmClick", BindingFlags.Instance | BindingFlags.NonPublic)
                    .Invoke(form, new object[] { null, EventArgs.Empty });
                Application.DoEvents();
                Assert(
                    button.Enabled == (mappingSaved && published),
                    "Add-to-solution requires an actual saved mapping and confirmed publication"
                );
                if (button.Enabled)
                {
                    typeof(ReviewForm)
                        .GetMethod(
                            "AddToSolutionClick",
                            BindingFlags.Instance | BindingFlags.NonPublic
                        )
                        .Invoke(form, new object[] { button, EventArgs.Empty });
                    Application.DoEvents();
                    Assert(
                        solutionCalls == 1
                            && ReferenceEquals(solutionOwner, form)
                            && form.DialogResult == DialogResult.None,
                        "Opening solution selection must keep save results visible and must not repeat saving"
                    );
                    Capture(form, "save-modal-solution-preview.png");
                }
            }
        }
        using (
            var form = new ReviewForm(
                "Resultado da solution",
                new[] { new OperationResult { Success = true } }
            )
        )
        {
            Assert(
                !((Button)Field(form, "addToSolution")).Visible,
                "Result-only dialogs must not offer the save-modal solution action"
            );
        }
    }

    private sealed class RecordedProgress : IProgress<MappingProgress>
    {
        public List<MappingProgress> Events { get; } = new List<MappingProgress>();

        public void Report(MappingProgress progress)
        {
            Events.Add(progress);
        }
    }

    private static void SaveProgress()
    {
        var fake = new FakeDataverse { FailTarget = "y" };
        var service = new MappingService(fake);
        var workspace = service.Load(fake.Map);
        var first = new MappingRow { Source = "a", Target = "x" };
        var second = new MappingRow { Source = "b", Target = "y" };
        workspace.Rows.AddRange(new[] { first, second });
        var progress = new RecordedProgress();
        var results = service.SaveAndPublish(workspace, progress);
        foreach (var row in new[] { first, second })
        {
            var events = progress.Events.Where(item => ReferenceEquals(item.Mapping, row)).ToList();
            Assert(
                events.Count == 2 && events[0].State == "Em execução",
                "Service must report start before terminal state for each mapping"
            );
        }
        Assert(
            progress.Events.Any(item => item.Mapping == first && item.State == "Salvo"),
            "Service must report saved mapping"
        );
        Assert(
            progress.Events.Any(item => item.Mapping == second && item.State == "Falha"),
            "Service must report failed mapping"
        );
        Assert(
            results.Any(result => result.Operation == "Publicação" && result.Success),
            "Publication must be reported separately"
        );
    }

    private static byte[] CreateSolutionZip(string xml)
    {
        using (var stream = new MemoryStream())
        {
            using (var zip = new ZipArchive(stream, ZipArchiveMode.Create, true))
            using (var writer = new StreamWriter(zip.CreateEntry("customizations.xml").Open()))
            {
                writer.Write(xml);
            }
            return stream.ToArray();
        }
    }

    private sealed class FakeDataverse : IOrganizationService
    {
        public TableMap Map { get; } =
            new TableMap
            {
                Id = Guid.NewGuid(),
                Source = "source",
                Target = "target",
            };
        public EntityMetadata Source { get; } = Table("source", Text("a"), Text("b"), Text("c"));
        public EntityMetadata Target { get; } = Table("target", Text("x"), Text("y"), Text("z"));
        public List<Entity> Records { get; private set; } = new List<Entity>();
        public List<OrganizationRequest> SolutionRequests { get; } =
            new List<OrganizationRequest>();
        public string FailTarget { get; set; }
        public bool FailPublish { get; set; }
        public bool LoseResponse { get; set; }
        public bool OmitExportMappings { get; set; }
        public bool OmitSystemMappings { get; set; }
        public bool FailExport { get; set; }
        public int TransactionCount { get; private set; }

        public void Add(string source, string target)
        {
            var record = new Entity("attributemap", Guid.NewGuid());
            record["sourceattributename"] = source;
            record["targetattributename"] = target;
            record["issystem"] = false;
            record["ismanaged"] = false;
            Records.Add(record);
        }

        public OrganizationResponse Execute(OrganizationRequest request)
        {
            var metadata = request as RetrieveEntityRequest;
            if (metadata != null)
            {
                var response = new RetrieveEntityResponse();
                response.Results["EntityMetadata"] =
                    metadata.LogicalName == "source" ? Source : Target;
                return response;
            }
            var transaction = request as ExecuteTransactionRequest;
            if (transaction != null)
            {
                TransactionCount++;
                var candidate = Records.ToList();
                var responses = new OrganizationResponseCollection();
                foreach (var operation in transaction.Requests)
                {
                    var delete = operation as DeleteRequest;
                    if (delete != null)
                    {
                        candidate.RemoveAll(record => record.Id == delete.Target.Id);
                        responses.Add(new DeleteResponse());
                        continue;
                    }
                    var create = (CreateRequest)operation;
                    var target = create.Target.GetAttributeValue<string>("targetattributename");
                    if (target == FailTarget)
                    {
                        throw new FaultException<OrganizationServiceFault>(
                            new OrganizationServiceFault
                            {
                                Message = "Injected destination failure: " + target,
                            }
                        );
                    }
                    if (
                        candidate.Any(record =>
                            record.GetAttributeValue<string>("targetattributename") == target
                        )
                    )
                    {
                        throw new FaultException<OrganizationServiceFault>(
                            new OrganizationServiceFault { Message = "Duplicate destination" }
                        );
                    }
                    var created = new Entity("attributemap", Guid.NewGuid());
                    created.Attributes.AddRange(create.Target.Attributes);
                    created["ismanaged"] = false;
                    created["issystem"] = false;
                    candidate.Add(created);
                    var response = new CreateResponse();
                    response.Results["id"] = created.Id;
                    responses.Add(response);
                }
                Records = candidate;
                if (LoseResponse)
                {
                    throw new TimeoutException("Response lost after commit");
                }
                var result = new ExecuteTransactionResponse();
                result.Results["Responses"] = responses;
                return result;
            }
            if (request.RequestName == "PublishXml")
            {
                if (FailPublish)
                {
                    throw new InvalidOperationException("Injected publication failure");
                }
                return new OrganizationResponse();
            }
            if (request.RequestName == "ExportSolution")
            {
                if (FailExport)
                {
                    throw new InvalidOperationException("Missing export privileges");
                }
                var maps = new XElement("EntityMaps");
                if (!OmitExportMappings)
                {
                    maps.Add(
                        new XElement(
                            "EntityMap",
                            new XElement("EntitySource", Source.LogicalName),
                            new XElement("EntityTarget", Target.LogicalName),
                            new XElement(
                                "AttributeMaps",
                                Records
                                    .Where(record =>
                                        !OmitSystemMappings
                                        || !record.GetAttributeValue<bool>("issystem")
                                    )
                                    .Select(record => new XElement(
                                        "AttributeMap",
                                        new XElement(
                                            "AttributeSource",
                                            record.GetAttributeValue<string>("sourceattributename")
                                        ),
                                        new XElement(
                                            "AttributeTarget",
                                            record.GetAttributeValue<string>("targetattributename")
                                        )
                                    ))
                            )
                        )
                    );
                }
                var response = new OrganizationResponse();
                response.Results["ExportSolutionFile"] = CreateSolutionZip(
                    new XDocument(new XElement("ImportExportXml", maps)).ToString()
                );
                return response;
            }
            if (request.RequestName == "AddSolutionComponent")
            {
                var type = (int)request["ComponentType"];
                if (type == 46 || type == 47)
                {
                    throw new InvalidOperationException(
                        "Subcomponent cannot be added because the root component is missing"
                    );
                }
                SolutionRequests.Add(request);
                return new OrganizationResponse();
            }
            throw new NotSupportedException(request.RequestName);
        }

        public EntityCollection RetrieveMultiple(QueryBase query)
        {
            return new EntityCollection(Records.ToList());
        }

        public Guid Create(Entity entity)
        {
            throw new NotSupportedException();
        }

        public void Update(Entity entity)
        {
            throw new NotSupportedException();
        }

        public void Delete(string entityName, Guid id)
        {
            throw new NotSupportedException();
        }

        public Entity Retrieve(string entityName, Guid id, ColumnSet columnSet)
        {
            throw new NotSupportedException();
        }

        public void Associate(
            string entityName,
            Guid entityId,
            Relationship relationship,
            EntityReferenceCollection relatedEntities
        )
        {
            throw new NotSupportedException();
        }

        public void Disassociate(
            string entityName,
            Guid entityId,
            Relationship relationship,
            EntityReferenceCollection relatedEntities
        )
        {
            throw new NotSupportedException();
        }
    }
}

# Engineering rules for this solution

These rules apply to every current and future project in this repository. This solution is the home of all
XrmToolBox tools maintained by Lucas Verissimo, so changes must preserve consistency across tools.

## Required working approach

1. Read this file and `ARCHITECTURE.md` before changing code.
2. Inspect the existing implementation and nearby projects before choosing a pattern.
3. Reuse the established solution pattern unless there is a documented reason to improve it everywhere.
4. Keep changes scoped, readable, debuggable, and independently testable.
5. Build every affected project in Debug and Release before considering work complete.
6. Run `dotnet tool restore` and `dotnet csharpier check .` after changing C# code.

## C# readability

- Follow the root `.editorconfig` and the repository CSharpier tool manifest.
- Never place multiple statements, declarations, conditions, or control-flow branches on one line.
- Use four-space indentation and Allman braces.
- Use descriptive names. One-letter names are acceptable only for conventional, very small scopes.
- Prefer guard clauses and small focused methods over deeply nested logic.
- Avoid compressed ternary chains when ordinary control flow is easier to debug.
- Do not mix UI construction, event handling, Dataverse access, and business logic in one method.
- Public and shared APIs must validate their arguments and document non-obvious behavior.

## Windows Forms standard

Every visual `Form` or `UserControl` must use the standard Visual Studio WinForms structure:

```text
FeatureControl.cs
FeatureControl.Designer.cs
FeatureControl.resx
```

- The main `.cs` file contains behavior, state transitions, event handlers, and calls to services.
- The `.Designer.cs` file contains `components`, `Dispose`, `InitializeComponent`, control declarations,
  layout, visual properties, and event wiring only.
- The `.resx` file is linked with `DependentUpon` in the project file.
- The class must be `partial`; a form inherits `Form` and a plugin control inherits the appropriate
  XrmToolBox control in the main file.
- Use named event-handler methods. Do not put anonymous business logic inside `InitializeComponent`.
- Do not manually move business rules into a designer file. Designer files must remain safe for the Visual
  Studio designer to regenerate.
- Verify that the designer's required assemblies are explicit project references and are copied to the output.
- Openability in the Visual Studio designer is part of the acceptance criteria for UI changes.

## Project boundaries

- Each XrmToolBox tool has its own project, assembly identity, version, NuGet specification, icon, and release
  process.
- A tool must not reference another tool project.
- Shared, reusable behavior belongs in `LucasVerissimo.XrmToolBox.Shared` only when it is genuinely useful to
  more than one tool and does not depend on tool-specific UI or rules.
- `Shared` must never reference a tool project.
- Keep UI models, scanners, monitoring rules, settings, and component-specific navigation in the owning tool.
- Prefer services/BLL classes for reusable Dataverse metadata, query, and data-access operations.

## Shared Project standard

XrmToolBox loads plugin assemblies from a common directory and its portal validates every assembly in the
package. Therefore:

- Shared code is maintained in `LucasVerissimo.XrmToolBox.Shared.shproj` and its `.projitems` file.
- Tool projects import the `.projitems`; Shared source is compiled directly into each tool assembly.
- Do not recreate a Shared `.csproj`, `ProjectReference`, or standalone Shared assembly.
- Do not remove or change shared members without evaluating and rebuilding every tool in the solution.
- Do not publish `LucasVerissimo.XrmToolBox.Shared.dll` as a separate file in a tool package.
- Each tool package must contain exactly one assembly directly under `lib/net48/Plugins`, and that assembly's
  version must match the NuGet package version.

## Validation checklist

Before handing work back:

- C# formatting check passes.
- Debug and Release builds pass for every affected project.
- No new compiler warnings are introduced.
- WinForms files have `.cs`, `.Designer.cs`, and `.resx` with correct `DependentUpon` metadata.
- Designer code contains no business logic.
- Shared logic is not duplicated in tool projects.
- One tool's package does not accidentally include another tool's assembly.
- A tool assembly contains the expected Shared Project types and no external reference to a Shared assembly.
- A tool package contains exactly one versioned plugin assembly under `lib/net48/Plugins`.
- Existing tools remain buildable and independently publishable.

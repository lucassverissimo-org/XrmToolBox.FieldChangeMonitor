using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using LucasVerissimo.XrmToolBox.DataverseMappingManager.Models;

namespace LucasVerissimo.XrmToolBox.DataverseMappingManager.Services
{
    /// <summary>
    /// Confirms mapping content in an exported solution, rather than treating an AddSolutionComponent response as transport proof.
    /// No archive or XML is modified, imported or written to disk.
    /// </summary>
    internal static class SolutionMappingExportVerifier
    {
        public static List<OperationResult> Verify(
            byte[] archiveBytes,
            IEnumerable<SolutionComponentItem> checks
        )
        {
            if (archiveBytes == null || archiveBytes.Length == 0)
            {
                throw new InvalidOperationException(
                    "O Dataverse não retornou o arquivo da solution."
                );
            }
            if (checks == null)
            {
                throw new ArgumentNullException(nameof(checks));
            }
            using (var stream = new MemoryStream(archiveBytes))
            using (var archive = new ZipArchive(stream, ZipArchiveMode.Read))
            {
                var entry = archive.Entries.SingleOrDefault(item =>
                    string.Equals(
                        item.FullName,
                        "customizations.xml",
                        StringComparison.OrdinalIgnoreCase
                    )
                );
                if (entry == null)
                {
                    throw new InvalidOperationException(
                        "A exportação não contém customizations.xml."
                    );
                }
                using (var content = entry.Open())
                using (
                    var reader = XmlReader.Create(
                        content,
                        new XmlReaderSettings
                        {
                            DtdProcessing = DtdProcessing.Prohibit,
                            XmlResolver = null,
                        }
                    )
                )
                {
                    var document = XDocument.Load(reader);
                    if (document.Root?.Name.LocalName != "ImportExportXml")
                    {
                        throw new InvalidOperationException(
                            "Estrutura de customizations.xml não reconhecida."
                        );
                    }
                    return checks.Select(check => VerifyMapping(document.Root, check)).ToList();
                }
            }
        }

        private static OperationResult VerifyMapping(XElement root, SolutionComponentItem check)
        {
            var expected =
                check.ExportExpectation
                ?? throw new ArgumentException(
                    "A verificação exige um par de tabelas e colunas.",
                    nameof(check)
                );
            var found = root.Elements()
                .Where(item => item.Name.LocalName == "EntityMaps")
                .SelectMany(section =>
                    section.Elements().Where(item => item.Name.LocalName == "EntityMap")
                )
                .Where(map =>
                    Equal(Value(map, "EntitySource"), expected.SourceTable)
                    && Equal(Value(map, "EntityTarget"), expected.TargetTable)
                )
                .SelectMany(map =>
                    map.Elements().Where(item => item.Name.LocalName == "AttributeMaps")
                )
                .SelectMany(section =>
                    section.Elements().Where(item => item.Name.LocalName == "AttributeMap")
                )
                .Any(map =>
                    Equal(Value(map, "AttributeSource"), expected.SourceColumn)
                    && Equal(Value(map, "AttributeTarget"), expected.TargetColumn)
                );
            if (!expected.ShouldExist)
            {
                return new OperationResult
                {
                    Operation = check.Label,
                    State = found ? "Falha" : "Atenção",
                    Detail = found
                        ? "O vínculo removido ainda aparece no pacote exportado. Não considere a remoção transportada."
                        : "O vínculo não aparece no pacote. Essa ausência não exclui um mapeamento em outro ambiente; a remoção no destino exige o processo de upgrade da solution. Não foi validada nenhuma importação.",
                };
            }
            if (!found && (expected.IsSystem || expected.IsManaged))
            {
                var origin = expected.IsSystem ? "de sistema" : "gerenciado";
                return new OperationResult
                {
                    Operation = check.Label,
                    State = expected.IsSystem ? "Nativo não exportado" : "Gerenciado não exportado",
                    Detail =
                        "Este vínculo foi identificado como "
                        + origin
                        + " no ambiente, mas não consta nesta exportação. "
                        + "A inclusão de tabelas e colunas não confirma o transporte desse vínculo. "
                        + "Confira o mapeamento e a solution de origem no ambiente de destino; a existência lá não foi verificada. "
                        + "Não exclua nem recrie o vínculo para forçar sua inclusão. O mapeamento no ambiente de origem permanece inalterado.",
                };
            }
            return new OperationResult
            {
                Operation = check.Label,
                Success = found,
                State = found ? "Verificado no pacote" : "Não confirmado",
                Detail = found
                    ? "Vínculo encontrado em EntityMaps/AttributeMaps do customizations.xml exportado. A importação em outro ambiente não foi testada."
                    : "O vínculo não apareceu no customizations.xml exportado. Revise a inclusão das duas tabelas, das colunas de origem/destino e do relacionamento na solution; depois tente novamente. O mapeamento salvo no ambiente não foi desfeito.",
            };
        }

        private static string Value(XElement parent, string name)
        {
            return parent
                .Elements()
                .FirstOrDefault(item => item.Name.LocalName == name)
                ?.Value.Trim();
        }

        private static bool Equal(string first, string second)
        {
            return string.Equals(first, second, StringComparison.OrdinalIgnoreCase);
        }
    }
}

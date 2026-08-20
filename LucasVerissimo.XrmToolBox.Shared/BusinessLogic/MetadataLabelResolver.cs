using System.Linq;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Metadata;

namespace LucasVerissimo.XrmToolBox.Shared.BusinessLogic
{
    /// <summary>
    /// Resolves localized metadata labels with a predictable logical-name fallback.
    /// </summary>
    public static class MetadataLabelResolver
    {
        public static string GetDisplayName(EntityMetadata entity)
        {
            return entity == null ? string.Empty : GetText(entity.DisplayName, entity.LogicalName);
        }

        public static string GetDisplayName(AttributeMetadata attribute)
        {
            return attribute == null
                ? string.Empty
                : GetText(attribute.DisplayName, attribute.LogicalName);
        }

        public static string GetText(Label label, string fallback)
        {
            var userLabel = label?.UserLocalizedLabel?.Label;
            if (!string.IsNullOrWhiteSpace(userLabel))
            {
                return userLabel;
            }

            var firstAvailableLabel = label
                ?.LocalizedLabels?.Select(localizedLabel => localizedLabel.Label)
                .FirstOrDefault(text => !string.IsNullOrWhiteSpace(text));

            return string.IsNullOrWhiteSpace(firstAvailableLabel)
                ? fallback ?? string.Empty
                : firstAvailableLabel;
        }
    }
}

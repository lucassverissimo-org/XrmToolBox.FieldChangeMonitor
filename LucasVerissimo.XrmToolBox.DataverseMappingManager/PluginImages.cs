namespace LucasVerissimo.XrmToolBox.DataverseMappingManager
{
    internal static class PluginImages
    {
        public const string SmallImageBase64 =
            "iVBORw0KGgoAAAANSUhEUgAAACAAAAAgCAYAAABzenr0AAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAAC9SURBVFhH7ZHBDYNADASvBiqhijzSVCR6SQcpJa3c+xAPS2S1e5gDEUB+zAd518M5df2z/JOEH44mBM4nkHOugvMMzCAh4BJ4DJ8fWFjB8qrjvAIKnGdgBrmnwKt8mzqkgOd+NQHMq44mgfJOcvluAgoTMIlpOROosVnAYMvXdmwWsNeYnwUziEtg6X7zv2cCmGcdzQLq6XcVUJgA5tZ2XFtgCcwgLgHP/RQsrzqkgALnGZhBQqAqcDQhEAIjX1Io3/67brgAAAAASUVORK5CYII=";
        public const string BigImageBase64 =
            "iVBORw0KGgoAAAANSUhEUgAAAFAAAABQCAYAAACOEfKtAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAAPVSURBVHhe7dlNiE1hGMDxKSullIXNlAUWkq/U1EgNEyEsSPKZkULyFYvZjJqw9RULJcqGJik0ig0LFkgmITUpC/JRJJthefTenOmc//vM07nnuZ17bz2LX2ruvM/z9m/GPZeOaYvWJq68Dn7B1ccDGnlAIw9o5AGNPKCRBzTygEYe0MgDGnlAo0IBZy/fmizbcqhh5q/pi3Y0AvdYcb6kUMDDg+eSsbGxhrly8160oxG4x4rzJR5QwfkSD6jgfIkHVHC+xAMqOF/iARWcL/GACs6XlA646tT9wni2qoC8h+bOsw/Rec6XeMD/PKAHtOMe3kNTecAzd0cK49mqAvIempejX6LznC8pHdCiqoBWnC/xgArOl3hABedLPKCC8yVtGbDn9Mlk76vh5MD7h7mvc48V90raLuBg8m6cB6zDnG37cvHaPiAfRDU8WyZgEILVE5D30FT+IM0LaHi2TMBZG3cnA39HPGA24PX+Gcnny5OTY33zop3EX98lAyeSVZfO5L6He3gPTVsGTG50jAsxuTe16fa1XDy+nuIe3kNTecCwsCieDQHDT102YBpx3fruaH82XojJ11Pcw3toRj//iM5zvqR0QIvs34GjF6dEIbtX9NRe6+zdnIt35OPj6G5Z3GPF+ZKmB+zsWh1F/HN9UvL63NTk+NenuYDhUYZ3y+IeK86XND1gKvzqZiO+f7IyF49vGBLuseJ8ScsETIV4Xx7MzcULH934fRLuseJ8ScsFDIY+XcgF/DE8s/YGs6Cnt/b6RO/Y3GPF+ZKWC8hHFr7B9K5eWvuT5wLuseJ8ScsF7P/1YjzezkdDUcDUrs2Lo7PcY8X5ktIB+SCq4dmJAmZ/8oL06/u3L4wC3hrojM5zD++hkZ5XOV/SMgHDG0U2Hh9Z0l/dLM7gHt5D09YBwyNKNl7X8f7oDmcPz44ChmfF7PdwD++haduA07s31D5hpPHCwzP3F8U9vIem8oBvPn4vjGezAbPxgvDxjfuL4h7eQ/Pt5+/oPOdLSge0SAPO23MwF0/7h4IiuMeK8yVNDdjIeAH3WHG+pGkB+cAcfhq5t17cY8X5kqYFzP7/Bh9ZyuIeK86XNC1gmBvChTeR8E7MnWVwjxXnS5oasNG4x4rzJR5QwfmS0gH5IKrh2aoC8h6ayh+keQENz3rAOi/Hsx6wzsvxrAc0qiqgFedLPKCC8yUeUMH5Eg+o4HyJB1RwvsQDKjhf4gEVnC8pFHDH0ZPJ85G3DTN4/mq0oxG4x4rzJYUCuol5QCMPaOQBjTygkQc08oBGHtDIAxp5QCMPaOQBjf4B/iFbccQzcRMAAAAASUVORK5CYII=";

        public static System.Drawing.Image CreateActionImage(string glyph)
        {
            if (string.IsNullOrWhiteSpace(glyph))
            {
                throw new System.ArgumentException("An action glyph is required.", nameof(glyph));
            }
            var image = new System.Drawing.Bitmap(20, 20);
            using (var graphics = System.Drawing.Graphics.FromImage(image))
            using (var font = new System.Drawing.Font("Segoe MDL2 Assets", 12F))
            using (
                var brush = new System.Drawing.SolidBrush(System.Drawing.SystemColors.ControlText)
            )
            using (
                var format = new System.Drawing.StringFormat
                {
                    Alignment = System.Drawing.StringAlignment.Center,
                    LineAlignment = System.Drawing.StringAlignment.Center,
                }
            )
            {
                graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;
                graphics.DrawString(
                    glyph,
                    font,
                    brush,
                    new System.Drawing.RectangleF(0, 0, 20, 20),
                    format
                );
            }
            return image;
        }

        public static System.Drawing.Icon CreateIcon(string encodedIcon)
        {
            if (string.IsNullOrWhiteSpace(encodedIcon))
            {
                throw new System.ArgumentException(
                    "An icon resource is required.",
                    nameof(encodedIcon)
                );
            }
            using (
                var stream = new System.IO.MemoryStream(
                    System.Convert.FromBase64String(encodedIcon)
                )
            )
            using (var icon = new System.Drawing.Icon(stream))
            {
                return (System.Drawing.Icon)icon.Clone();
            }
        }
    }
}

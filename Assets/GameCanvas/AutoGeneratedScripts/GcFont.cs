/*------------------------------------------------------------*/
// <summary>GameCanvas for Unity</summary>
// <author>Seibe TAKAHASHI</author>
// <remarks>
// (c) 2015-2021 Smart Device Programming.
// This software is released under the MIT License.
// http://opensource.org/licenses/mit-license.php
// </remarks>
/*------------------------------------------------------------*/
#nullable enable

namespace GameCanvas
{
    public readonly partial struct GcFont : System.IEquatable<GcFont>
    {
        internal const int __Length__ = 2;
        public static readonly GcFont DefaultFont = new GcFont("GcFontDefaultFont");
        public static readonly GcFont Fnt0 = new GcFont("GcFontFnt0");
    }
}

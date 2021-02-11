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
    public readonly partial struct GcSound : System.IEquatable<GcSound>
    {
        internal const int __Length__ = 2;
        public static readonly GcSound Snd0 = new GcSound("GcSoundSnd0");
        public static readonly GcSound Snd1 = new GcSound("GcSoundSnd1");
    }
}

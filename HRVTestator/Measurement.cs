using System;

namespace HRVTestator
{
    /// <summary>
    /// Die Klasse <see cref="Measurement"/> ist ein Data-transver-Object und ist verantwortlich für das Kapseln eines Messresultates.
    /// </summary>
    public class Measurement
    {
        /// <summary>
        /// Definition des Enums der Phasen des Experimentes.
        /// </summary>
        public enum EnumPhase { Pre, Exp, Post}

        /// <summary>
        /// Der HRV-Wert der Messung.
        /// </summary>
        public float HRV { get; set; }

        /// <summary>
        /// Der RR-Wert der Messung.
        /// </summary>
        public int RR { get; set; }

        /// <summary>
        /// Die Gültigkeit der Messung
        /// </summary>
        public bool IsValid { get; set; }

        /// <summary>
        /// Der Zeitpunkt der Messung.
        /// </summary>
        public DateTime DateTime { get; set; }

        /// <summary>
        /// Die Phase des Experimentes in welcher der Wert erhoben wurde.
        /// </summary>
        public EnumPhase Phase { get; set; }
    }
}
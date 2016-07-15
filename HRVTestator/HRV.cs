using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HRVTestator
{
    public class HRV
    {
        private List<Measurement> mesures = new List<Measurement>();
        private List<int> heartRateValues = new List<int>();
        private int amountOfValuesToCalculateHRV;
        private int lastRRValue = 0;
        private Measurement.EnumPhase currentPhase;

        public void SetPhase(Measurement.EnumPhase phase)
        {
            currentPhase = phase;
        }

        public string GetFormatedMesurement()
        {
            var sb = new StringBuilder();
            sb.AppendLine("UV: " + amountOfValuesToCalculateHRV);
            sb.AppendLine(" Vorhermessung");

            Measurement.EnumPhase currentPhase = Measurement.EnumPhase.Pre;

            foreach (Measurement mesure in mesures)
            {
                if (mesure.HRV == 0)
                {
                    continue;
                }

                if (mesure.Phase != currentPhase)
                {
                    sb.AppendLine(GetPhaseText(mesure.Phase));
                    currentPhase = mesure.Phase;
                }

                int lastRRValueForPrint = mesure.RR;

                if (mesure.IsValid)
                {
                    sb.AppendLine(mesure.DateTime + "   ; " + mesure.RR + "    ; " + mesure.HRV);
                }
                else
                {
                    sb.AppendLine(mesure.DateTime + ", Ungüliger Wert: Letzter Wert " + lastRRValueForPrint + " Aktueller Wert: " + mesure.RR);
                }
            }

            return sb.ToString();
        }

        private string GetPhaseText(Measurement.EnumPhase phase)
        {
            switch (phase)
            {
                case Measurement.EnumPhase.Post:
                    return " Nachhermessung";

                case Measurement.EnumPhase.Exp:
                    return " Exp-Phase";
            }

            throw new InvalidOperationException("Invalid EnumPhase value: " + phase);
        }

        public void SetAmountOfValuesToCalutateHRV(int amountOfValuesToCalculateHRV)
        {
            this.amountOfValuesToCalculateHRV = amountOfValuesToCalculateHRV;
        }

        public IEnumerable<Measurement> SetRR(int[] rrValues)
        {
            List<Measurement> tempMesures = new List<Measurement>();
            foreach (int rrValue in rrValues)
            {
                lastRRValue = rrValue;
                bool isValid = IsValueValid(rrValue);
                tempMesures.Add(new Measurement
                {
                    DateTime = DateTime.UtcNow,
                    IsValid = isValid,
                    RR = rrValue,
                    HRV = isValid ? CalulateHRV(rrValue) : default(int),
                    Phase = currentPhase,
                });

                mesures.AddRange(tempMesures);
            }

            return tempMesures.Where(x => x.IsValid);
        }

        public bool IsAmountOfValuesToCalculateHRVSet()
        {
            return amountOfValuesToCalculateHRV != 0;
        }

        public bool IsValueValid(int actualValue)
        {
            return !(lastRRValue - actualValue > 100 || actualValue - lastRRValue > 100);
        }

        private float CalulateHRV(int rrValue)
        {
            heartRateValues.Add(rrValue);
            float hrv = 0;
            List<float> squaredDiffOfNeigbours = new List<float>();

            if (heartRateValues.Count >= amountOfValuesToCalculateHRV + 1)
            {
                int negativCounter = -1 * ((int)amountOfValuesToCalculateHRV);
                for (int i = negativCounter; i < 0; i++)
                {
                    // Jedes Einzelne Differenzpaar berechnen           
                    float squaredDiffOfNeighbours = (heartRateValues[heartRateValues.Count + (i)]) - (heartRateValues[heartRateValues.Count + (i - 1)]);
                    squaredDiffOfNeigbours.Add(squaredDiffOfNeighbours);
                }

                float squaredSum = 0;
                // Die Summe der einzelnen quadrierten Differenzpaare berechnen
                foreach (float squaredDiffOfNeighbour in squaredDiffOfNeigbours)
                {
                    squaredSum += (float)Math.Pow(squaredDiffOfNeighbour, 2);
                }

                // Die Wurzel daraus ziehen
                hrv = (float)Math.Sqrt((1 / ((float)amountOfValuesToCalculateHRV - 1)) * squaredSum); ;
                Console.Out.WriteLine("HRV:....... " + String.Format("{0:0.00000}", hrv));
                heartRateValues.RemoveAt(0);
            }

            return hrv;
        }
    }

}
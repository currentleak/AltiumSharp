using System;
using System.Drawing;
using AltiumSharp.BasicTypes;

namespace AltiumSharp.Records
{

    public class PieChartRecord : SchPrimitive
    {
        public CoordPoint Location { get; internal set; }
        public Coord Radius { get; internal set; }
        public LineWidth LineWidth { get; internal set; }
        public double StartAngle { get; internal set; }
        public double EndAngle { get; internal set; }
        public Color Color { get; internal set; }
        public Color AreaColor { get; internal set; }
        public bool IsSolid { get; internal set; }

        public override CoordRect CalculateBounds() =>
            new CoordRect(Location.X - Radius, Location.Y - Radius, Radius * 2, Radius * 2);

        public override void ImportFromParameters(ParameterCollection p)
        {
            if (p == null) throw new ArgumentNullException(nameof(p));

            base.ImportFromParameters(p);
            Location = new CoordPoint(
                Utils.DxpFracToCoord(p["LOCATION.X"].AsIntOrDefault(), p["LOCATION.X_FRAC"].AsIntOrDefault()),
                Utils.DxpFracToCoord(p["LOCATION.Y"].AsIntOrDefault(), p["LOCATION.Y_FRAC"].AsIntOrDefault()));
            Radius = Utils.DxpFracToCoord(p["RADIUS"].AsIntOrDefault(), p["RADIUS_FRAC"].AsIntOrDefault());
            LineWidth = (LineWidth)p["LINEWIDTH"].AsIntOrDefault();
            StartAngle = p["STARTANGLE"].AsDoubleOrDefault();
            EndAngle = p["ENDANGLE"].AsDoubleOrDefault();
            Color = p["COLOR"].AsColorOrDefault();
            AreaColor = p["AREACOLOR"].AsColorOrDefault();
            IsSolid = p["ISSOLID"].AsBool();
        }

        public override void ExportToParameters(ParameterCollection p)
        {
            if (p == null) throw new ArgumentNullException(nameof(p));

            base.ExportToParameters(p);
            {
                var (n, f) = Utils.CoordToDxpFrac(Location.X);
                if (n != 0 || f != 0) p.Add("LOCATION.X", n);
                if (f != 0) p.Add("LOCATION.X" + "_FRAC", f);
            }
            {
                var (n, f) = Utils.CoordToDxpFrac(Location.Y);
                if (n != 0 || f != 0) p.Add("LOCATION.Y", n);
                if (f != 0) p.Add("LOCATION.Y" + "_FRAC", f);
            }
            {
                var (n, f) = Utils.CoordToDxpFrac(Radius);
                if (n != 0 || f != 0) p.Add("RADIUS", n);
                if (f != 0) p.Add("RADIUS" + "_FRAC", f);
            }
            p.Add("LINEWIDTH", (int)LineWidth);
            p.Add("STARTANGLE", StartAngle);
            p.Add("ENDANGLE", EndAngle);
            p.Add("COLOR", Color);
            p.Add("AREACOLOR", AreaColor);
            p.Add("ISSOLID", IsSolid);
        }
    }
}
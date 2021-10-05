using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using DlibDotNet;

namespace FeaturesExtraction
{
    public class Face
    {
        private Dictionary<int, Point> _points;
        public Face()
        {
            _points = new Dictionary<int, Point>();
        }

        public void AddPoint(int id, Point point)
        {
            //Console.WriteLine(id + " " + point.X + " " + point.Y);
            _points.Add(id,point);
        }

        public double LeftEyebrow()
        {
            var result1 = GetDistance(_points[22].X, _points[40].X, _points[22].Y, _points[40].Y);
            var result2 = GetDistance(_points[21].X, _points[40].X, _points[21].Y, _points[40].Y);
            var result3 = GetDistance(_points[20].X, _points[40].X, _points[20].Y, _points[40].Y);
            var result4 = GetDistance(_points[19].X, _points[40].X, _points[19].Y, _points[40].Y);


            var d1 = result4 / result1;
            var d2 = result3 / result1;
            var d3 = result2 / result1;
            var d4 = result1 / result1;

            return d1 + d2 + d3 + d4;
        }

        public double RightEyebrow()
        {
            var result1 = GetDistance(_points[23].X, _points[43].X, _points[23].Y, _points[43].Y);
            var result2 = GetDistance(_points[24].X, _points[43].X, _points[24].Y, _points[43].Y);
            var result3 = GetDistance(_points[25].X, _points[43].X, _points[25].Y, _points[43].Y);
            var result4 = GetDistance(_points[26].X, _points[43].X, _points[26].Y, _points[43].Y);

            var d1 = result4 / result1;
            var d2 = result3 / result1;
            var d3 = result2 / result1;
            var d4 = result1 / result1;

            return d1 + d2 + d3 + d4;


        }

        public double LeftLip()
        {
            var result1 = GetDistance(_points[49].X, _points[34].X, _points[49].Y, _points[34].Y);
            var result2 = GetDistance(_points[50].X, _points[34].X, _points[50].Y, _points[34].Y);
            var result3 = GetDistance(_points[51].X, _points[34].X, _points[51].Y, _points[34].Y);
            var result4 = GetDistance(_points[52].X, _points[34].X, _points[52].Y, _points[34].Y);

            var d1 = result4 / result4;
            var d2 = result3 / result4;
            var d3 = result2 / result4;
            var d4 = result1 / result4;

            return d1 + d2 + d3 + d4;

        }

        public double RightLip()
        {
            var result1 = GetDistance(_points[55].X, _points[34].X, _points[55].Y, _points[34].Y);
            var result2 = GetDistance(_points[54].X, _points[34].X, _points[54].Y, _points[34].Y);
            var result3 = GetDistance(_points[53].X, _points[34].X, _points[53].Y, _points[34].Y);
            var result4 = GetDistance(_points[52].X, _points[34].X, _points[52].Y, _points[34].Y);

            var d1 = result4 / result4;
            var d2 = result3 / result4;
            var d3 = result2 / result4;
            var d4 = result1 / result4;

            return d1 + d2 + d3 + d4;

        }

        public double LipWidth()
        {
            var result1 = GetDistance(_points[49].X, _points[55].X, _points[49].Y, _points[55].Y);
            var result2 = GetDistance(_points[34].X, _points[52].X, _points[34].Y, _points[52].Y);
            var d3 = result1 / result2;

            return d3;


        }


        public double LipHeight()
        {
            var result1 = GetDistance(_points[52].X, _points[58].X, _points[52].Y, _points[58].Y);
            var result2 = GetDistance(_points[34].X, _points[52].X, _points[34].Y, _points[52].Y);
            var d3 = result1 / result2;

            return d3 ;


        }


        public double Eye()
        {
            var result1 = GetDistance(_points[34].X, _points[43].X, _points[34].Y, _points[43].Y);
            var result2 = GetDistance(_points[34].X, _points[48].X, _points[34].Y, _points[48].Y);
            var result3 = GetDistance(_points[34].X, _points[47].X, _points[34].Y, _points[47].Y);
            var result4 = GetDistance(_points[34].X, _points[45].X, _points[34].Y, _points[45].Y);

            var d1 = result4 / result1;
            var d2 = result3 / result1;
            var d3 = result2 / result1;
            var d4 = result1 / result1;

            return d1 + d2 + d3 + d4;

        }

        public double OpenMouth()
        {
            var result1 = GetDistance(_points[34].X, _points[63].X, _points[34].Y, _points[63].Y);
            var result2 = GetDistance(_points[34].X, _points[67].X, _points[34].Y, _points[67].Y);
            var result = result2/result1;
            return result;
        }

        public double MouthLine()
        {
            var result1 = GetDistance(_points[34].X, _points[55].X, _points[34].Y, _points[55].Y);
            var result2 = GetDistance(_points[9].X, _points[55].X, _points[9].Y, _points[55].Y);
            var result = result2 / result1;
            return result;
        }

        public static double GetDistance(int x1, int x2, int y1, int y2)
        {
            int distX = x1 - x2;
            int distY = y1 - y2;

            double result = Math.Sqrt(Math.Pow(distX, 2) + Math.Pow(distY, 2));
            return result;
        }

        public override string ToString()
        {
            return LeftEyebrow() + ", " + RightEyebrow() + ", " + LeftLip() + ", " + RightLip() + ", " + LipWidth() + ", " + LipHeight();
        }

        public String getBasicString()
        {

            return LeftEyebrow() + ", " + RightEyebrow() + ", " + LeftLip() + ", " + RightLip() + ", " + LipWidth() + ", " + LipHeight();

        }

        public String getExtraString()
        {
            return LeftEyebrow() + ", " + RightEyebrow() + ", " + LeftLip() + ", " + RightLip() + ", " + LipWidth() + ", " + LipHeight() + "," + Eye() + "," + OpenMouth() + "," + MouthLine();
        }
    }
}

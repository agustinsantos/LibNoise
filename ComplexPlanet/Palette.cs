using LibNoise;

namespace ComplexPlanet
{
    public static class Palette
    {
        public static GradientColor TOPO_15LEV = new GradientColor();
        public static GradientColor TEMP_19LEV = new GradientColor();
        public static GradientColor HOTCOLR_19LEV = new GradientColor();
        public static GradientColor PERC2_9LEV = new GradientColor();

        static Palette()
        {
            // Addapted from MeteoSwiss NCL library
            TOPO_15LEV.AddGradientPoint(-1000, new Color(40, 54, 154));
            TOPO_15LEV.AddGradientPoint(0, new Color(7, 106, 127));
            TOPO_15LEV.AddGradientPoint(200, new Color(0, 201, 50));
            TOPO_15LEV.AddGradientPoint(400, new Color(30, 211, 104));
            TOPO_15LEV.AddGradientPoint(600, new Color(94, 224, 116));
            TOPO_15LEV.AddGradientPoint(800, new Color(162, 235, 130));
            TOPO_15LEV.AddGradientPoint(1000, new Color(223, 248, 146));
            TOPO_15LEV.AddGradientPoint(1200, new Color(246, 229, 149));
            TOPO_15LEV.AddGradientPoint(1400, new Color(200, 178, 118));
            TOPO_15LEV.AddGradientPoint(1600, new Color(162, 126, 94));
            TOPO_15LEV.AddGradientPoint(1800, new Color(143, 97, 84));
            TOPO_15LEV.AddGradientPoint(2000, new Color(162, 125, 116));
            TOPO_15LEV.AddGradientPoint(2200, new Color(178, 150, 139));
            TOPO_15LEV.AddGradientPoint(2400, new Color(199, 176, 170));
            TOPO_15LEV.AddGradientPoint(2600, new Color(219, 205, 202));
            TOPO_15LEV.AddGradientPoint(2800, new Color(236, 228, 226));
            TOPO_15LEV.AddGradientPoint(10000, new Color(255, 255, 255));

            TEMP_19LEV.AddGradientPoint(220, new Color(7, 30, 70));
            TEMP_19LEV.AddGradientPoint(230, new Color(7, 47, 107));
            TEMP_19LEV.AddGradientPoint(235, new Color(8, 82, 156));
            TEMP_19LEV.AddGradientPoint(240, new Color(33, 113, 181));
            TEMP_19LEV.AddGradientPoint(245, new Color(66, 146, 199));
            TEMP_19LEV.AddGradientPoint(250, new Color(90, 160, 205));
            TEMP_19LEV.AddGradientPoint(255, new Color(120, 191, 214));
            TEMP_19LEV.AddGradientPoint(260, new Color(170, 220, 230));
            TEMP_19LEV.AddGradientPoint(265, new Color(219, 245, 255));
            TEMP_19LEV.AddGradientPoint(270, new Color(240, 252, 255));
            TEMP_19LEV.AddGradientPoint(275, new Color(255, 240, 245));
            TEMP_19LEV.AddGradientPoint(280, new Color(255, 224, 224));
            TEMP_19LEV.AddGradientPoint(285, new Color(252, 187, 170));
            TEMP_19LEV.AddGradientPoint(290, new Color(252, 146, 114));
            TEMP_19LEV.AddGradientPoint(295, new Color(251, 106, 74));
            TEMP_19LEV.AddGradientPoint(300, new Color(240, 60, 43));
            TEMP_19LEV.AddGradientPoint(305, new Color(204, 24, 30));
            TEMP_19LEV.AddGradientPoint(310, new Color(166, 15, 20));
            TEMP_19LEV.AddGradientPoint(315, new Color(120, 10, 15));
            TEMP_19LEV.AddGradientPoint(320, new Color(95, 0, 0));

            HOTCOLR_19LEV.AddGradientPoint(0, new Color(0, 0, 50));
            HOTCOLR_19LEV.AddGradientPoint(1 * 13, new Color(24, 24, 112));
            HOTCOLR_19LEV.AddGradientPoint(2 * 13, new Color(16, 78, 139));
            HOTCOLR_19LEV.AddGradientPoint(3 * 13, new Color(23, 116, 205));
            HOTCOLR_19LEV.AddGradientPoint(4 * 13, new Color(72, 118, 255));
            HOTCOLR_19LEV.AddGradientPoint(5 * 13, new Color(91, 172, 237));
            HOTCOLR_19LEV.AddGradientPoint(6 * 13, new Color(173, 215, 230));
            HOTCOLR_19LEV.AddGradientPoint(7 * 13, new Color(209, 237, 237));
            HOTCOLR_19LEV.AddGradientPoint(8 * 13, new Color(229, 239, 249));
            HOTCOLR_19LEV.AddGradientPoint(9 * 13, new Color(242, 255, 255));
            HOTCOLR_19LEV.AddGradientPoint(10 * 13, new Color(253, 245, 230));
            HOTCOLR_19LEV.AddGradientPoint(11 * 13, new Color(255, 228, 180));
            HOTCOLR_19LEV.AddGradientPoint(12 * 13, new Color(243, 164, 96));
            HOTCOLR_19LEV.AddGradientPoint(13 * 13, new Color(237, 118, 0));
            HOTCOLR_19LEV.AddGradientPoint(14 * 13, new Color(205, 102, 29));
            HOTCOLR_19LEV.AddGradientPoint(15 * 13, new Color(224, 49, 15));
            HOTCOLR_19LEV.AddGradientPoint(16 * 13, new Color(237, 0, 0));
            HOTCOLR_19LEV.AddGradientPoint(17 * 13, new Color(205, 0, 0));
            HOTCOLR_19LEV.AddGradientPoint(18 * 13, new Color(139, 0, 0));
            HOTCOLR_19LEV.AddGradientPoint(19 * 13, new Color(50, 0, 0));

            PERC2_9LEV.AddGradientPoint(0 * 26, new Color(215, 227, 238));
            PERC2_9LEV.AddGradientPoint(1 * 26, new Color(181, 202, 255));
            PERC2_9LEV.AddGradientPoint(2 * 26, new Color(143, 179, 255));
            PERC2_9LEV.AddGradientPoint(3 * 26, new Color(127, 151, 255));
            PERC2_9LEV.AddGradientPoint(4 * 26, new Color(171, 207, 99));
            PERC2_9LEV.AddGradientPoint(5 * 26, new Color(232, 245, 158));
            PERC2_9LEV.AddGradientPoint(6 * 26, new Color(255, 250, 20));
            PERC2_9LEV.AddGradientPoint(7 * 26, new Color(255, 209, 33));
            PERC2_9LEV.AddGradientPoint(8 * 26, new Color(255, 163, 10));
            PERC2_9LEV.AddGradientPoint(9 * 26, new Color(255, 76, 0));
        }
    }
}

using System;
using System.Drawing;

namespace WindowsPrinter
{
    /// <summary>
    /// Вспомогательный класс для формирования изображения ШК
    /// </summary>
    public static class BarcodeBuilder
    {
        #region Правила формирования ШК стандарта EAN13

        private const int ODD = 0;
        private const int EVEN = 1;
        private const int RIGHT = 2;

        private static string[][] EAN_CHARSET = new string[][] {
            new string[] {"0001101", "0100111", "1110010"},
            new string[] {"0011001", "0110011", "1100110"},
            new string[] {"0010011", "0011011", "1101100"}, 
            new string[] {"0111101", "0100001", "1000010"},
            new string[] {"0100011", "0011101", "1011100"},
            new string[] {"0110001", "0111001", "1001110"},
            new string[] {"0101111", "0000101", "1010000"},
            new string[] {"0111011", "0010001", "1000100"},
            new string[] {"0110111", "0001001", "1001000"},
            new string[] {"0001011", "0010111", "1110100"}};

        private static int[][] EAN_PARITY = new int[][] {
            new int[] {ODD, ODD,  ODD,  ODD,  ODD,  ODD },
            new int[] {ODD, ODD,  EVEN, ODD,  EVEN, EVEN},
            new int[] {ODD, ODD,  EVEN, EVEN, ODD,  EVEN},
            new int[] {ODD, ODD,  EVEN, EVEN, EVEN, ODD },
            new int[] {ODD, EVEN, ODD,  ODD,  EVEN, EVEN},
            new int[] {ODD, EVEN, EVEN, ODD,  ODD,  EVEN},
            new int[] {ODD, EVEN, EVEN, EVEN, ODD,  ODD },
            new int[] {ODD, EVEN, ODD,  EVEN, ODD,  EVEN},
            new int[] {ODD, EVEN, ODD,  EVEN, EVEN, ODD },
            new int[] {ODD, EVEN, EVEN, ODD,  EVEN, ODD }
        };

        #endregion

        /// <summary>
        /// Преобразование строки штрихкода в изображение
        /// </summary>
        /// <param name="barcode">Строка ШК</param>
        /// <param name="height">Высота изображения в пикселях</param>
        /// <returns></returns>
        public static Bitmap GetBarcodeImage(string barcode, int height)
        {
            // расчет контрольной суммы            
            char[] barcodeChars = barcode.ToCharArray(0, 12);
            int nChecksum = 0;
            for (int i = 0; i < 12; i += 2)
            {
                nChecksum += Convert.ToInt32(barcodeChars[i].ToString());
                if (i + 1 < 12)
                    nChecksum += Convert.ToInt32(barcodeChars[i + 1].ToString()) * 3;
            }
            nChecksum = nChecksum % 10;
            if (nChecksum > 0)
                nChecksum = 10 - nChecksum;
            barcode += nChecksum.ToString();

            // формирование ШК

            // стартовая последовательность
            string barcodeBits = "101";
            // первая половина ШК
            int systemCode = Convert.ToInt32(barcode[0].ToString());
            for (int i = 1; i < 7; i++)
            {
                int value = Convert.ToInt32(barcode[i].ToString());
                barcodeBits += EAN_CHARSET[value][EAN_PARITY[systemCode][i - 1]];
            }
            // разделитель
            barcodeBits += "01010";
            // вторая половина ШК
            for (int i = 7; i < 13; i++)
            {
                int value = Convert.ToInt32(barcode[i].ToString());
                barcodeBits += EAN_CHARSET[value][RIGHT];
            }
            // завершающая последовательность
            barcodeBits += "101";

            // запись картинки
            var bitmap = new Bitmap(barcodeBits.Length, height);
            for (int y = 0; y < height; y++)
                for (int x = 0; x < barcodeBits.Length; x++)
                    bitmap.SetPixel(x, y, barcodeBits[x] == '1' ? Color.Black : Color.White);

            return bitmap;
        }
    }
}

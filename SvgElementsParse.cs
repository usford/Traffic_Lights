using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml;
using System.Runtime;
using System.Text.RegularExpressions;

namespace Traffic_Lights {
    public class SvgElementsParse {
        public List<XamlButtons> GetCoordinatesButtons() {
            var map = new List<XamlButtons>();
            var fileElements = Directory.GetFiles($@"{MainWindow.pathDirectory}\Элементы схемы\Elements");
            var xDoc = new XmlDocument();
            var listUniqueElements = new List<string>();

            foreach (var fileElement in fileElements) {
                xDoc.Load((@$"{fileElement}"));
                var layers = xDoc.DocumentElement.ChildNodes;
                string idElement = Path.GetFileName(fileElement.Split('.')[0]);
                string uniqueIdElement = String.Concat(idElement.SkipLast(1));
                if (uniqueIdElement.StartsWith("kn")) {
                    if (!listUniqueElements.Contains(uniqueIdElement)) {
                        listUniqueElements.Add(uniqueIdElement);

                        foreach (XmlNode layer in layers) {
                            if (layer.Attributes["inkscape:label"] != null) {
                                if (layer.Attributes["style"].Value == $"display:inline") {
                                    {
                                        CoordinatesButtons coordinates =  GetCoordinate(layer);
                                        map.Add(new XamlButtons(
                                            coordinates,
                                            idElement));
                                    }
                                }
                            }
                        }
                    }   
                }     
            }
            
            foreach (XamlButtons m in map) {
                Console.WriteLine(m.ID);
                Console.WriteLine(m.Coordinates.x + ":" + m.Coordinates.y);
            }
            return map;
        }

        public CoordinatesButtons GetCoordinate(XmlNode layer) {
            //Console.WriteLine(layer.Attributes["inkscape:label"].Value);
            var map = new CoordinatesButtons();
            double x = 0.0;
            double y = 0.0;

            foreach (XmlNode layerElement in layer.ChildNodes) {
                if (layer.Attributes["transform"] != null) {
                    CoordinatesButtons transformParent = ParseRegularExpressionTranslate(layer.Attributes["transform"].Value);
                    x += transformParent.x;
                    y += transformParent.y;
                }

                if (layerElement.Attributes["transform"] != null) {
                    if (layerElement.Attributes["transform"].Value.StartsWith("translate")) {
                        CoordinatesButtons transformG = ParseRegularExpressionTranslate(layerElement.Attributes["transform"].Value);
                        x += transformG.x;
                        y += transformG.y;

                        foreach (XmlNode layerElementChild in layerElement.ChildNodes) {
                            if (layerElementChild.Name == "rect") {
                                x += Convert.ToDouble(layerElementChild.Attributes["x"].Value.Replace('.', ','));
                                y += Convert.ToDouble(layerElementChild.Attributes["y"].Value.Replace('.', ','));
                            }
                        }
                    }
                    if (layerElement.Attributes["transform"].Value.StartsWith("matrix")) {

                        foreach (XmlNode layerElementChild in layerElement.ChildNodes) {
                            if (layerElementChild.Name == "rect") {
                                CoordinatesButtons transformG = ParseRegularExpressionMatrix(
                                    layerElement.Attributes["transform"].Value,
                                    Convert.ToDouble(layerElementChild.Attributes["x"].Value.Replace('.', ',')),
                                    Convert.ToDouble(layerElementChild.Attributes["y"].Value.Replace('.', ',')));

                                x += transformG.x;
                                y += transformG.y;
                            }
                        }
                    }
                }

            }

            //Преобразование из координат svg (275.161) в координаты окна 1060/840
            double coefX = 1060.0d / 200.0d;
            double coefY = 840.0d / 188.0d;

            map.x = x * coefX + 60;
            map.y = y * coefY;

            //13 слой должен быть X:19,093683000000002, Y:145,21376
            //7 слой должен быть X:61,283645, Y:145,431952
            //9 слой должен быть X:102,71008461708671, Y:145,51123706026
            //11 слой должен быть X:142,2192612869467, Y:145,17064105789999
            //Console.WriteLine($"X:{x}, Y:{y}");

            return map;
        }

        public CoordinatesButtons ParseRegularExpressionTranslate(string regularExpression) {
            var map = new CoordinatesButtons();
            double x = 0.0;
            double y = 0.0;
            //Console.WriteLine(regularExpression);
            var doubleNumberPattern = @"(-\d|\d)+?(?:[.]\d+?)?"; ;
            var regexTranslateXY = new Regex($@"^translate\({doubleNumberPattern},{doubleNumberPattern}\)\z");
            var regexTranslateX = new Regex($@"^translate\({doubleNumberPattern}\)\z");

            //Регулярное выражение типа translate(x,y)
            if (regexTranslateXY.IsMatch(regularExpression)) {
                x += Convert.ToDouble(Regex.Matches(regularExpression, @$"{doubleNumberPattern}\b")[0].ToString().Replace('.', ','));
                y += Convert.ToDouble(Regex.Matches(regularExpression, @$"\b{doubleNumberPattern}\b")[1].ToString().Replace('.', ','));
            }

            //Регулярное выражение типа translate(x)
            if (regexTranslateX.IsMatch(regularExpression)) {
                x += Convert.ToDouble(Regex.Matches(regularExpression, @$"{doubleNumberPattern}\b")[0].ToString().Replace('.', ','));
            }

            map.x = x;
            map.y = y;
            return map;
        }
        public CoordinatesButtons ParseRegularExpressionMatrix(string regularExpression, double rectX, double rectY) {
            var map = new CoordinatesButtons();
            double x = 0.0;
            double y = 0.0;
            double a = 0.0;
            //Console.WriteLine(regularExpression);
            var doubleNumberPattern = @"(-\d|\d)+?(?:[.]\d+?)?"; ;
            var regexMatrix= new Regex($@"^matrix\({doubleNumberPattern},{doubleNumberPattern},{doubleNumberPattern},{doubleNumberPattern},{doubleNumberPattern},{doubleNumberPattern}\)\z");

            //Регулярное выражение типа matrix(a,b,c,d,e,f)
            if (regexMatrix.IsMatch(regularExpression)) {
                //Console.WriteLine("Матрица");
                a += Convert.ToDouble(Regex.Matches(regularExpression, @$"{doubleNumberPattern}\b")[0].ToString().Replace('.', ','));
                x += Convert.ToDouble(Regex.Matches(regularExpression, @$"{doubleNumberPattern}\b")[4].ToString().Replace('.', ','));
                y += Convert.ToDouble(Regex.Matches(regularExpression, @$"{doubleNumberPattern}\b")[5].ToString().Replace('.', ','));
            }

            map.x = rectX * a + x;
            map.y = rectY * a + y;
            return map;
        }
        // id элемента - наименование слоя
        public Dictionary<string, string> GetElementsMap() {
            var map = new Dictionary<string, string>();
            var fileElements = Directory.GetFiles($@"{MainWindow.pathDirectory}\Элементы схемы\Elements");
            var xDoc = new XmlDocument();

            foreach (var fileElement in fileElements) {
                xDoc.Load((@$"{fileElement}"));
                var layers = xDoc.DocumentElement.ChildNodes;

                foreach (XmlNode layer in layers) {
                    if (layer.Attributes["inkscape:label"] != null) {
                        if (layer.Attributes["style"].Value == $"display:inline") {
                            string idElement = Path.GetFileName(fileElement.Split('.')[0]);
                            var layerName = layer.Attributes["inkscape:label"].Value;
                            map.Add(idElement, layerName);
                        }      
                    }
                }
            }
            return map;
        }

        public struct XamlButtons {
            public CoordinatesButtons Coordinates;
            public string ID;

            public XamlButtons(CoordinatesButtons coordinates, string id) {
                Coordinates = coordinates;
                ID = id;
            }
        }
        public struct CoordinatesButtons {
            public double x;
            public double y;
        }
    }
}

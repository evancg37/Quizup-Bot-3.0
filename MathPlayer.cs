using System;
using System.Collections.Generic;
using System.Linq;
using AForge.Imaging;
using System.Drawing;

namespace QuizupBot_3._0
{
    class MathPlayer
    {
        const float TEMPLATE_MATCHING_SIMILARITY = 0.0f;
        const float CHARACTER_EPSILON = 0.01f;
        const int ANSWER_CHARACTER_SPACING = 5;
        const int DISPLAY_CHARACTER_HEIGHT = 150;
        const int ANSWER_CHARACTER_HEIGHT = 60;

        const string CHARACTER_DIRECTORY = @"D:\_programming\Quizupbot 3.0\CHARACTERS\";

        static int count = 0;

        static Size DISPLAY_SIZE = new Size(1080, DISPLAY_CHARACTER_HEIGHT);
        static Size ANSWER_SIZE = new Size(1080, ANSWER_CHARACTER_HEIGHT);

        public static Rectangle QUESTION_DISPLAY = new Rectangle(new Point(0, 410), DISPLAY_SIZE);

        public static Rectangle ANSWER_1 = new Rectangle(new Point(0,  780), ANSWER_SIZE);
        public static Rectangle ANSWER_2 = new Rectangle(new Point(0, 1050), ANSWER_SIZE);
        public static Rectangle ANSWER_3 = new Rectangle(new Point(0, 1320), ANSWER_SIZE);
        public static Rectangle ANSWER_4 = new Rectangle(new Point(0, 1590), ANSWER_SIZE);

        static Dictionary<string, Bitmap> displayBitmaps = new Dictionary<string, Bitmap>
        {
            ["1"] = new Bitmap(CHARACTER_DIRECTORY + "display1.bmp"),
            ["2"] = new Bitmap(CHARACTER_DIRECTORY + "display2.bmp"),
            ["3"] = new Bitmap(CHARACTER_DIRECTORY + "display3.bmp"),
            ["4"] = new Bitmap(CHARACTER_DIRECTORY + "display4.bmp"),
            ["5"] = new Bitmap(CHARACTER_DIRECTORY + "display5.bmp"),
            ["6"] = new Bitmap(CHARACTER_DIRECTORY + "display6.bmp"),
            ["7"] = new Bitmap(CHARACTER_DIRECTORY + "display7.bmp"),
            ["8"] = new Bitmap(CHARACTER_DIRECTORY + "display8.bmp"),
            ["9"] = new Bitmap(CHARACTER_DIRECTORY + "display9.bmp"),
            ["0"] = new Bitmap(CHARACTER_DIRECTORY + "display0.bmp"),
            ["-"] = new Bitmap(CHARACTER_DIRECTORY + "display-.bmp"),
            ["/"] = new Bitmap(CHARACTER_DIRECTORY + "displayD.bmp"),
            ["="] = new Bitmap(CHARACTER_DIRECTORY + "display=.bmp"),
        };

        static Dictionary<string, Bitmap> answerBitmaps = new Dictionary<string, Bitmap>
        {
            ["1"] = new Bitmap(CHARACTER_DIRECTORY + "answer1.bmp"),
            ["2"] = new Bitmap(CHARACTER_DIRECTORY + "answer2.bmp"),
            ["3"] = new Bitmap(CHARACTER_DIRECTORY + "answer3.bmp"),
            ["4"] = new Bitmap(CHARACTER_DIRECTORY + "answer4.bmp"),
            ["5"] = new Bitmap(CHARACTER_DIRECTORY + "answer5.bmp"),
            ["6"] = new Bitmap(CHARACTER_DIRECTORY + "answer6.bmp"),
            ["7"] = new Bitmap(CHARACTER_DIRECTORY + "answer7.bmp"),
            ["8"] = new Bitmap(CHARACTER_DIRECTORY + "answer8.bmp"),
            ["9"] = new Bitmap(CHARACTER_DIRECTORY + "answer9.bmp"),
            ["0"] = new Bitmap(CHARACTER_DIRECTORY + "answer0.bmp"),
            ["-"] = new Bitmap(CHARACTER_DIRECTORY + "answer-.bmp"),
        };

        static public List<MathCharacter> decipherMathFromArea(Bitmap area, bool answerMode=false)
        {
            List<MathCharacter> mathList = new List<MathCharacter>();
            Rectangle SCANNER_BAR;
            Point upperLeft = new Point(0, 0);
            Size characterSize;

            if (answerMode)
            {
                SCANNER_BAR = new Rectangle(new Point(0, 0), new Size(1, ANSWER_CHARACTER_HEIGHT));
                characterSize = new Size(34, ANSWER_CHARACTER_HEIGHT);
            }else
            {
                SCANNER_BAR = new Rectangle(new Point(0, 0), new Size(1, DISPLAY_CHARACTER_HEIGHT)); //0, 410, stretches down 150.
                characterSize = new Size(54, DISPLAY_CHARACTER_HEIGHT);
            }

            Bitmap section;
            int scannerX = 0;
            bool whitePixelFound = false, foundOnLastLoop = false;

            while (scannerX < 1080)
            {
                section = area.Clone(SCANNER_BAR, area.PixelFormat);

                //SCREENSHOTTING AT EVERY 100
                /*if (scannerX % 50 == 0 && scannerX > 400)
                {
                    Console.WriteLine("Do you want to take a screen shot at {0}, {1}? (y / *)", scannerX, 0);
                    var pressedKey = Console.ReadKey().KeyChar;
                    Console.Read();
                    if (pressedKey == 'y')
                    {
                        string saveStr = string.Format(@"C:\Users\evanc\Desktop\section-{0}.bmp", scannerX);
                        section.Save(saveStr);
                    }

                    Console.WriteLine("Press any key to continue through the image scanning process.");
                    pressedKey = Console.ReadKey().KeyChar;
                    if (pressedKey == 'x')
                        break;
                }
                */

                foundOnLastLoop = whitePixelFound;
                whitePixelFound = checkIfWhiteInArea(section, invert: answerMode, DEBUG: false);

                if (whitePixelFound && !foundOnLastLoop) //This is the beginning of white pixels!
                {
                    upperLeft.X = scannerX;
                }
                else if (!whitePixelFound && foundOnLastLoop) //This is the edge of the white pixels!
                {
                    //Rectangle completed. Get bitmap from rectangle and scan it for numbers and operators, add the result to the list.

                    Rectangle resultRectangle = new Rectangle(upperLeft, characterSize);
                    var characterSection = area.Clone(resultRectangle, area.PixelFormat);

                    MathCharacter foundCharacter = findCharacterInArea(characterSection, answerMode: answerMode);

                    if (foundCharacter.number == "=")
                    {
                        break;
                    }
                    else if (foundCharacter.number != "?")
                        mathList.Add(foundCharacter);

                    //CHARACTER CUTTING

                    count++;
                    string saveChar = foundCharacter.number;
                    if (saveChar.Equals("/"))
                        saveChar = "X"; 
                    string savePath = String.Format(CHARACTER_DIRECTORY + "CHARACTER_" + count + ".bmp");
                    characterSection.Save(savePath);

                    foundOnLastLoop = false;
                          

                }

                scannerX += 1; SCANNER_BAR.X = scannerX;
            }

            mathList.Add(new MathCharacter("="));
            return mathList;

        }

        static bool checkIfWhiteInArea(Bitmap section, bool invert=false, bool DEBUG=false)
        {
            bool whitePixelFound = false;
            int pixel;

            for (int i = 0; i < section.Height; i++)
            {
                pixel = section.GetPixel(0, i).ToArgb();

                if (DEBUG)
                    Console.WriteLine("Pixels at {0:D2}: {1}", i, pixel);

                if ((pixel == -1 ) && ! invert) //&& ! invert
                {
                    whitePixelFound = true;
                    break;
                }
                else if (pixel != -1 && invert)
                {
                    whitePixelFound = true;
                    break;
                }
            }
            return whitePixelFound;
        }

        static MathCharacter findCharacterInArea(Bitmap section, bool answerMode=false)
        {
            DateTime methodCall = DateTime.Now;
            ExhaustiveTemplateMatching tm = new ExhaustiveTemplateMatching(TEMPLATE_MATCHING_SIMILARITY);

            string foundCharacter = "?";

            Dictionary<float, string> characterComparison = new Dictionary<float, string>();
            Bitmap characterBitmap;
            bool lookupWorked;

            //build a dictionary out of all possible characters and bitmap paths to those characters to look at, depends on mode.
            Dictionary<string, Bitmap> characterBitmaps;
            if (answerMode)
                characterBitmaps = answerBitmaps;
            else
                characterBitmaps = displayBitmaps;

            foreach (string keyCharacter in characterBitmaps.Keys)
            {
                lookupWorked = characterBitmaps.TryGetValue(keyCharacter, out characterBitmap);

                if (! lookupWorked)
                    Console.WriteLine("The lookup of Bitmap for key {0} was unsuccessful.", keyCharacter);

                else
                {
                    TemplateMatch[] matches = tm.ProcessImage(section, characterBitmap);

                    if (matches.Count() > 0) //Should always be true. There has to be at least some similarity between every single character.
                    {
                        characterComparison.Add(matches[0].Similarity, keyCharacter);
                    }
                }
            }

            float maxFound = characterComparison.Keys.Max();

            characterComparison.TryGetValue(maxFound, out foundCharacter);
            //Console.WriteLine("The decided on character to export is: {0}", foundCharacter);

            return (new MathCharacter(foundCharacter));
        }

        public static int calculateMathFromList(List<MathCharacter> mathList)
        {
            Console.WriteLine("Calculating math. List says: ");
            foreach (MathCharacter character in mathList)
                Console.Write("{0} ", character.number);
            Console.WriteLine();

            List<int> mathNumbers = new List<int>();
            List<MathCharacter> mathOperators = new List<MathCharacter>();

            string result = "";

            List<MathCharacter>.Enumerator numerator = mathList.GetEnumerator();
            MathCharacter current;
            MathCharacter previous = new MathCharacter("0");

            //Special case for first character being a negative sign.
            numerator.MoveNext();
            if (numerator.Current.isNegate)
            {
                result += "-";
                numerator.MoveNext();
            }

            //583 / 11 = comes in.

            while (numerator.Current != null) //loop that fills array numbers with 1 FULL number at a time including negative signs. //operators contains operators in order.
            {
                current = numerator.Current;

                if (current.isNumber())
                {
                    result += current.number;
                }

                else
                {
                    if (previous.isOperator() && current.isNegate)  //Like in 743[+-]12
                        result += "-";                              //-..12 = -12

                    else
                    {
                        int parsedNumber;
                        try
                        {
                            parsedNumber = int.Parse(result);
                            result = "";
                            mathNumbers.Add(parsedNumber);
                            mathOperators.Add(current);
                        }
                        catch (FormatException ex)
                        {
                            Console.WriteLine("The processing failed for data {0}", result);
                            result = "";
                        }
                    }
                }

                previous = current;
                numerator.MoveNext();
            }

            //End array creation loop

            //THIS PROGRAM IS SO FAR INCAPABLE OF PERFORMING MULTIPLE OPERATIONS PER LIST

            int number1 = -99999, number2 = -99999;

            if (mathNumbers.Count() == 0)
            {
                Console.WriteLine("There is no math that can be completed for list in calculateMathFromList()");
                return number1;
            }
            else
            {
                number1 = mathNumbers.ElementAt(0);
            }

            if (mathNumbers.Count() > 1)
                number2 = mathNumbers.ElementAt(1);

            if (mathOperators.ElementAt(0).isEqualSign)
                return number1;

            if (mathOperators.ElementAt(0).isDivisor)
                return (number1 / number2);

            else if (mathOperators.ElementAt(0).isAdder)
                return (number1 + number2);

            else if (mathOperators.ElementAt(0).isNegate)
                return (number1 - number2);

            return -99999;
        }

        /* public Bitmap renderAnswerBitmap(int answer)
        {
            string answerString = answer.ToString();
            int approxWidth = answerString.Length * 30 + (answerString.Length - 1 * ANSWER_CHARACTER_SPACING);
            Bitmap result = new Bitmap(approxWidth, 60);
            int currentX = 0;

            foreach (char c in answerString.ToList())
            {
                string bitmapPath;
                if (!answerBitmapPaths.TryGetValue(c.ToString(), out bitmapPath))
                    Console.WriteLine("The answer character lookup for character {0} was unsuccessful.", c.ToString());

                else
                {
                    Bitmap answerCharacter = new Bitmap(bitmapPath);
                    currentX += (30 + ANSWER_CHARACTER_SPACING);

                    using (Graphics g = Graphics.FromImage(result))
                    {
                        g.DrawImage(result, 0, 0);
                        Rectangle spacer = new Rectangle(new Point(currentX, 0), new Size(ANSWER_CHARACTER_SPACING, 60));
                        g.DrawRectangle(new Pen(Color.White), spacer);
                        g.DrawImage(answerCharacter, result.Width + ANSWER_CHARACTER_SPACING, 0);
                    }
                }
            }
            result.Save(@"C:\Users\evanc\Desktop\ANSWER_RENDER_" + answer + ".bmp");
            return result;
        } */

        public static MathAnswers decipherMathAnswers(Bitmap screen, int solution)
        {
            int answer1, answer2, answer3, answer4;
            Bitmap answershot1 = getBitmapFromRectangle(screen, ANSWER_1);
            Bitmap answershot2 = getBitmapFromRectangle(screen, ANSWER_2);
            Bitmap answershot3 = getBitmapFromRectangle(screen, ANSWER_3);
            Bitmap answershot4 = getBitmapFromRectangle(screen, ANSWER_4);

            answer1 = calculateMathFromList(decipherMathFromArea(answershot1, answerMode: true));
            answer2 = calculateMathFromList(decipherMathFromArea(answershot2, answerMode: true));
            answer3 = calculateMathFromList(decipherMathFromArea(answershot3, answerMode: true));
            answer4 = calculateMathFromList(decipherMathFromArea(answershot4, answerMode: true));

            return (new MathAnswers(solution, answer1, answer2, answer3, answer4));
        }

        public static Bitmap getBitmapFromRectangle(Bitmap source, Rectangle rect)
        {
            return source.Clone(rect, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
        }

        public static MathResult solveProblem(Bitmap problem)
        {
            DateTime callTime = DateTime.Now;
            Bitmap answerShot = new Bitmap(problem);

            List<MathCharacter> mathList = decipherMathFromArea(getBitmapFromRectangle(problem, QUESTION_DISPLAY));

            /*
            result.appendLog("Problem mathList: ", newline: false);
            foreach (MathCharacter character in mathList)
                result.appendLog(character.number, newline: false);
            result.appendLog("");
            */

            int answer = calculateMathFromList(mathList);
            //result.appendLog("Answered successfully retrieved: " + answer);

            MathAnswers answerSet = decipherMathAnswers(problem, answer);
            //result.setAnswer(answerSet);

            using (Graphics g = Graphics.FromImage(answerShot))
            {
                g.DrawImage(problem, 0, 0);
                g.DrawString(answerSet.answer.ToString(), new Font("Arial", 48), new SolidBrush(Color.Aqua), new PointF(505, 600));
                Point curvePoint = new Point(answerSet.answerPoint.X-270, answerSet.answerPoint.Y-30);
                Size rectangleSize = new Size(540, 60);

                Pen pen;
                if (! answerSet.answerFound)                        //Red circle if guess
                    pen = new Pen(Color.Red, 10.0f);
                else                                                //Green circle if correct
                    pen = new Pen(Color.Green, 10.0f);

                g.DrawEllipse(pen, new Rectangle(curvePoint, rectangleSize));
            }

            MathResult result = new MathResult(problem, answerShot, answerSet, DateTime.Now - callTime);
            return result;
        }

    }

    class MathAnswers
    {
        public readonly int answer;
        public readonly int choice1;
        public readonly int choice2;
        public readonly int choice3;
        public readonly int choice4;
        public readonly Point answerPoint;
        public readonly bool answerFound;

        public MathAnswers(int answer, int a, int b, int c, int d)
        {
            this.answer = answer;
            choice1 = a; choice2 = b; choice3 = c; choice4 = d;
            answerPoint = findAnswerPoint();
            answerFound = (answer == choice1 || answer == choice2 || answer == choice3 || answer == choice4);
        }

        public MathAnswers()
        {
            answer = -9999; choice1 = -9991; choice2 = -9992; choice3 = -9993; choice4 = -9994;
            answerPoint = findAnswerPoint();
            answerFound = false;
        }

        private Point findAnswerPoint()
        {
            Point point = new Point();

            if (choice1 == answer)
                point = new Point(540, MathPlayer.ANSWER_1.Location.Y + 30);
            else if (choice2 == answer)
                point = new Point(540, MathPlayer.ANSWER_2.Location.Y + 30);
            else if (choice3 == answer)
                point = new Point(540, MathPlayer.ANSWER_3.Location.Y + 30);
            else if (choice4 == answer)
                point = new Point(540, MathPlayer.ANSWER_4.Location.Y + 30);
            else
            {
                point = new Point(540, MathPlayer.ANSWER_3.Location.Y + 30);
                Console.WriteLine("Error in object MathAnswers.getAnswerPoint(), answer found not be found! Guessing C.");
            }

            return point;
        }
    }

    class MathResult
    {
        public readonly Bitmap problemScreenshot;
        public readonly Bitmap answerScreenshot;
        public readonly MathAnswers answers;
        public readonly TimeSpan executeTime;
        public string log;

        public MathResult(Bitmap problem, Bitmap solved, MathAnswers answerSet, TimeSpan span)
        {
            problemScreenshot = problem;
            answerScreenshot = solved;
            answers = answerSet;
            executeTime = span;
            log = "";
            executeTime = new TimeSpan(0, 0, 0);
        }

        public void appendLog(string s, bool newline=true)
        {
            if (newline)
                s += "\n";
            log += s;
        }
          
    }
    class MathCharacter
    {
        public bool isDivisor = false;
        public bool isEqualSign = false;
        public bool isNegate = false;
        public bool isAdder = false;
        public string number;

        public MathCharacter(string character)
        {
            if (character == "/")
            {
                isDivisor = true;
                this.number = "/";
            }
            else if (character == "=")
            {
                isEqualSign = true;
                this.number = "=";
            }
            else if (character == "+")
            {
                isAdder = true;
                this.number = "+";
            }
            else if (character == "-")
            {
                isNegate = true;
                this.number = "-";
            }
            else
            {
                this.number = character;
            }

        }

        public bool isOperator()
        {
            return (isDivisor || isNegate || isEqualSign || isAdder);
        }
        public bool isNumber()
        {
            return (!isOperator());
        }

    }

}

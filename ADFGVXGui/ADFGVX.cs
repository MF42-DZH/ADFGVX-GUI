using System;
using System.Collections.Generic;
using System.Linq;

namespace ADFGVXGui
{
    class ADFGVX
    {
        private string Keyword { get; set; }
        private string ShiftKeyword { get; set; }
        public string RawKeyword { get; set; }
        public string RawShiftKeyword { get; set; }
        string[][] BaseTable { get; set; }
        string[][] KeyedTable { get; set; }

        public ADFGVX(string MainKeyword, string KeywordForTransposition)
        {
            Keyword = RemoveDuplicates(RemoveNonAlphanumericCharacters(MainKeyword.ToUpperInvariant()));
            ShiftKeyword = RemoveDuplicates(RemoveNonAlphanumericCharacters(KeywordForTransposition.ToUpperInvariant()));

            RawKeyword = MainKeyword;
            RawShiftKeyword = KeywordForTransposition;

            BaseTable = CreateBaseTable();
            KeyedTable = CreateParsedKeywordTable(Keyword);
        }

        public string EncodeMessage(string Message)
        {
            Message = Message.ToUpperInvariant();

            string Output = "";

            foreach (char character in Message)
            {
                for (int y = 0; y < KeyedTable.Length; y++)
                {
                    for (int x = 0; x < KeyedTable[y].Length; x++)
                    {
                        if (KeyedTable[y][x] == character.ToString())
                        {
                            Output += BaseTable[y][x];
                            goto LoopContinue;
                        }
                    }
                }
                LoopContinue:;
            }

            Dictionary<string, List<string>> UnshiftedTable = CreateShiftDictionary(ShiftKeyword);

            for (int i = 0; i < Output.Length; i++)
            {
                UnshiftedTable[ShiftKeyword[i % ShiftKeyword.Length].ToString()].Add(Output[i].ToString());
            }

            Dictionary<string, List<string>> ShiftedTable = ColumnarTranspose(UnshiftedTable, true);

            string FinalOutput = "";

            foreach (string key in ShiftedTable.Keys)
            {
                foreach (string character in ShiftedTable[key])
                {
                    FinalOutput += character;
                }
            }

            return FinalOutput;
        }

        public string DecodeMessage(string Message)
        {
            Message = RemoveNonADFGVXCharacters(Message);

            string Output = "";
            string FinalOutput = "";

            Dictionary<string, int> ShiftTable = CreateValueTable(Message);
            Dictionary<string, int> FixedShiftTable = ColumnarTranspose(ShiftTable, true);
            Dictionary<string, List<string>> MessageTable = CreateShiftDictionary(ShiftKeyword);

            int currentPtr = 0;
            foreach (string key in FixedShiftTable.Keys)
            {
                for (int i = 0; i < FixedShiftTable[key]; i++)
                {
                    MessageTable[key].Add(Message[currentPtr].ToString());
                    currentPtr++;
                }
            }

            currentPtr = 0;
            while (true)
            {
                try
                {
                    foreach (string key in MessageTable.Keys)
                    {
                        Output += MessageTable[key][currentPtr];
                    }
                }
                catch (ArgumentOutOfRangeException)
                {
                    break;
                }
                currentPtr++;
            }

            for (int i = 0; i < Output.Length; i += 2)
            {
                string CurrentPair = Output[i].ToString() + Output[i + 1].ToString();

                int coordX = 0;
                int coordY = 0;

                for (int y = 0; y < BaseTable.Length; y++)
                {
                    for (int x = 0; x < BaseTable[y].Length; x++)
                    {
                        if (BaseTable[y][x] == CurrentPair)
                        {
                            coordX = x;
                            coordY = y;
                            goto LoopExit;
                        }
                    }
                }

                LoopExit:;
                FinalOutput += KeyedTable[coordY][coordX];
            }

            return FinalOutput;
        }

        public void ModifyKeywords(string MainKeyword, string KeywordForTransposition)
        {
            Keyword = RemoveDuplicates(RemoveNonAlphanumericCharacters(MainKeyword.ToUpperInvariant()));
            ShiftKeyword = RemoveDuplicates(RemoveNonAlphanumericCharacters(KeywordForTransposition.ToUpperInvariant()));

            RawKeyword = MainKeyword;
            RawShiftKeyword = KeywordForTransposition;

            KeyedTable = CreateParsedKeywordTable(Keyword);
        }

        protected string RemoveNonADFGVXCharacters(string str)
        {
            string FinalString = "";
            string[] Letters = { "A", "D", "F", "G", "V", "X" };

            foreach (char character in str)
            {
                if (Letters.Contains(character.ToString()))
                {
                    FinalString += character;
                }
            }

            return FinalString;
        }

        protected string RemoveNonAlphanumericCharacters(string str)
        {
            char[] AlphaNumSet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789".ToCharArray();
            string finalString = "";

            foreach (char character in str)
            {
                if (AlphaNumSet.Contains(character))
                {
                    finalString += character;
                }
            }

            return finalString;
        }

        protected string RemoveDuplicates(string str)
        {
            string strNoDuplicates = "";

            foreach (char character in str)
            {
                if (!strNoDuplicates.Contains(character))
                {
                    strNoDuplicates += character;
                }
            }

            return strNoDuplicates;
        }

        protected string[][] CreateBaseTable()
        {
            string[] Letters = { "A", "D", "F", "G", "V", "X" };
            string[][] EncodingArr = new string[6][];

            for (int i = 0; i < EncodingArr.Length; i++)
            {
                EncodingArr[i] = new string[6];
            }

            for (int y = 0; y < EncodingArr.Length; y++)
            {
                for (int x = 0; x < EncodingArr[y].Length; x++)
                {
                    EncodingArr[y][x] = Letters[y] + Letters[x];
                }
            }

            return EncodingArr;
        }

        protected string[][] CreateParsedKeywordTable(string UsedKeyword)
        {
            string AlphaNumSet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            string UsedAlphaNumSet = "";

            string[][] AlphaNumArr = new string[6][];

            for (int i = 0; i < AlphaNumArr.Length; i++)
            {
                AlphaNumArr[i] = new string[6];
            }

            int CurrentIndex = 0;
            for (int y = 0; y < AlphaNumArr.Length; y++)
            {
                for (int x = 0; x < AlphaNumArr[y].Length; x++)
                {
                    try
                    {
                        AlphaNumArr[y][x] = UsedKeyword[CurrentIndex].ToString();
                        UsedAlphaNumSet += UsedKeyword[CurrentIndex];
                        CurrentIndex++;
                    }
                    catch (IndexOutOfRangeException)
                    {
                        foreach (char character in AlphaNumSet)
                        {
                            if (!UsedAlphaNumSet.Contains(character))
                            {
                                AlphaNumArr[y][x] = character.ToString();
                                UsedAlphaNumSet += character;

                                break;
                            }
                        }
                    }
                }
            }

            return AlphaNumArr;
        }

        protected Dictionary<string, List<string>> CreateShiftDictionary(string UsedKeyword)
        {
            Dictionary<string, List<string>> FinalTable = new Dictionary<string, List<string>>();

            for (int i = 0; i < UsedKeyword.Length; i++)
            {
                FinalTable.Add(UsedKeyword[i].ToString(), new List<string>());
            }

            return FinalTable;
        }

        protected Dictionary<string, List<string>> ColumnarTranspose(Dictionary<string, List<string>> DictToApply, bool Mix)
        {
            Dictionary<string, List<string>> FinalDictionary = new Dictionary<string, List<string>>();

            List<char> UsedKeyword = ShiftKeyword.ToList();
            if (Mix)
            {
                UsedKeyword.Sort();
            }

            foreach (char key in UsedKeyword)
            {
                FinalDictionary.Add(key.ToString(), DictToApply[key.ToString()]);
            }

            return FinalDictionary;
        }

        protected Dictionary<string, int> ColumnarTranspose(Dictionary<string, int> DictToApply, bool Mix)
        {
            Dictionary<string, int> FinalDictionary = new Dictionary<string, int>();

            List<char> UsedKeyword = ShiftKeyword.ToList();
            if (Mix)
            {
                UsedKeyword.Sort();
            }

            foreach (char key in UsedKeyword)
            {
                FinalDictionary.Add(key.ToString(), DictToApply[key.ToString()]);
            }

            return FinalDictionary;
        }

        protected Dictionary<string, int> CreateValueTable(string Message)
        {
            Dictionary<string, int> ValueTable = new Dictionary<string, int>();
            int TotalMessageLength = Message.Length;

            int Remainder = TotalMessageLength % ShiftKeyword.Length;

            foreach (char character in ShiftKeyword)
            {
                ValueTable.Add(character.ToString(), (TotalMessageLength - Remainder) / ShiftKeyword.Length);
            }

            for (int x = 0; x < Remainder; x++)
            {
                ValueTable[ValueTable.Keys.ToArray()[x]]++;
            }

            return ValueTable;
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace BitCoinManagerModels.Identity
{
    public class Encryptor
    {
        private static string UPPER = "_u";
        private static string LOWER = "_l";

        public static string Encrypt(string decryption, CodeKey decodeKey)
        {
            var encryption = string.Empty;
            var temp = string.Empty;
            var addition = string.Empty;

            switch (decodeKey)
            {
                case CodeKey.oei_ntla_ltnb_si:
                    decryption.ToList().ForEach(c =>
                    {
                        var charString = $"{string.Empty}{c}";

                        if (Char.IsNumber(c))
                            addition = TranslatedNumber(charString, decodeKey);
                        else if (Char.IsLetter(c))
                            addition = TranslatedLetter(charString, decodeKey, incrementAbove: false);
                        else addition = charString;

                        temp += addition;
                    });
                    encryption = temp += (int)decodeKey;
                    break;
                case CodeKey.pi_oei_ntlb_ltna:
                    decryption.ToList().ForEach(c =>
                    {
                        var charString = $"{string.Empty}{c}";

                        if (Char.IsNumber(c))
                            addition = TranslatedNumber(charString, decodeKey, incrementAbove: false);
                        else if (Char.IsLetter(c))
                            addition = TranslatedLetter(charString, decodeKey);
                        else addition = charString;

                        temp += addition;
                    });
                    encryption = $"{(int)decodeKey} + {temp}";
                    break;
            }

            return encryption;
        }

        private static string TranslatedNumber(string charString, CodeKey codeKey, bool? captioned = null, bool incrementAbove = true, bool encrtypt = true)
        {
            var translationWithoutKey = Enum.GetValues(typeof(Translation)).Cast<Translation>().ToList().Find(t => (int)t == int.Parse(charString));
            var number = translationWithoutKey.ValueWithKey(codeKey, incrementAbove);
            var translation = Enum.GetValues(typeof(Translation)).Cast<Translation>().ToList().Find(e => (int)e == number);
            var translationWithCaptionVerification = captioned.HasValue ? (captioned.Value ? translation.ToString().ToUpper() : translation.ToString().ToLower()) : translation.ToString();
            var letter = encrtypt ? string.Join(",", Enumerable.Range(1, (int)codeKey).Select(n => translationWithCaptionVerification)) : translationWithCaptionVerification;
            return $"{Prefix(encrtypt)}{letter}{Suffix(encrtypt, charString)}";
        }
        private static string TranslatedLetter(string charString, CodeKey codeKey, bool incrementAbove = true, bool encrtypt = true)
        {
            var letter = charString;
            var translation = Enum.GetValues(typeof(Translation)).Cast<Translation>().ToList().Find(e => e.ToString() == letter.ToUpper());
            var valueWithKey = translation.ValueWithKey(codeKey, incrementAbove);
            var number = encrtypt ? string.Join(",", Enumerable.Range(1, (int)codeKey).Select(n => valueWithKey.ToString())) : valueWithKey.ToString();
            return $"{Prefix(encrtypt)}{number}{Suffix(encrtypt, charString)}";
        }

        private static string Prefix(bool encrtypt)
        {
            return encrtypt ? "#" : string.Empty;
        }
        private static string Suffix(bool encrtypt, string charString)
        {
            return encrtypt ? (charString.All(c => Char.IsLower(c)) ? LOWER : UPPER) : string.Empty;
        }

        //private static string EnumDescription(Translation translation)
        //{
        //    return ((DescriptionAttribute)translation.GetType().GetMember(translation.ToString())[0].GetCustomAttributes(typeof(DescriptionAttribute), false).ElementAt(0)).Description;
        //}

        public static string Decrypt(string encryption, CodeKey encodeKey)
        {
            var decryption = string.Empty;
            var temp = encryption.TrimEnd($"{(int)encodeKey}".ToArray());
            var distinctEncryption = new List<(string Value, string Caption)>();
            var tempSplit = temp.Split('#').ToList();
            foreach (var s in tempSplit)
            {
                var sSplit = s.Split('_').ToList();
                if (sSplit.Count() > 1)
                {
                    var caption = sSplit[1];
                    var symbols = string.Join(string.Empty, caption.Where(c => !Char.IsLetter(c) && !Char.IsNumber(c)));
                    if (!string.IsNullOrWhiteSpace(symbols))
                        caption = caption.Replace(symbols, string.Empty);
                    distinctEncryption.Add((sSplit[0].Split(',')[0], $"_{caption}"));
                    if (!string.IsNullOrWhiteSpace(symbols))
                        distinctEncryption.Add((symbols, LOWER));
                }
            }
            var addition = string.Empty;

            switch (encodeKey)
            {
                case CodeKey.oei_ntla_ltnb_si:
                    distinctEncryption.ForEach(c =>
                    {
                        var charString = $"{string.Empty}{c.Value}";
                        var IsCaptioned = c.Caption == UPPER ? true : false;

                        if (c.Value.All(sc => Char.IsNumber(sc)))
                            addition = TranslatedNumber(charString, encodeKey, captioned: IsCaptioned, encrtypt: false);
                        else if (c.Value.All(sc => Char.IsLetter(sc)))
                            addition = TranslatedLetter(charString, encodeKey, incrementAbove: false, encrtypt: false);
                        else addition = charString;

                        decryption += addition;
                    });
                    break;
                case CodeKey.pi_oei_ntlb_ltna:
                    distinctEncryption.ToList().ForEach(c =>
                    {
                        var charString = $"{string.Empty}{c.Value}";
                        var IsCaptioned = c.Caption == UPPER ? true : false;

                        if (c.Value.All(sc => Char.IsNumber(sc)))
                            addition = TranslatedNumber(charString, encodeKey, captioned: IsCaptioned, encrtypt: false);
                        else if (c.Value.All(sc => Char.IsLetter(sc)))
                            addition = TranslatedLetter(charString, encodeKey, incrementAbove: false, encrtypt: false);
                        else addition = charString;

                        decryption += addition;
                    });
                    break;
            }

            return decryption;
        }
    }

    public enum CodeKey
    {
        [Description("one equals {id}, number to letter ({id} times above), letter to number ({id} times bellow), suffix {id}")]
        oei_ntla_ltnb_si = 2,
        [Description("prefix {id}, one equals {id}, number to letter ({id} times bellow), letter to number ({id} times above)")]
        pi_oei_ntlb_ltna = 3
    }

    //[AttributeUsage(AttributeTargets.Field)]
    //internal sealed class EditableDescription : Attribute
    //{
    //}

    internal static class EnumExtensions
    {
        public static int ValueWithKey(this Translation translation, CodeKey key, bool incrementAbove = true)
        {
            var output = 0;
            var value = translation.ToString();
            var valueInt = (int)translation;
            var keyInt = (int)key;
            if (incrementAbove)
            {
                output = valueInt + keyInt;

                if (output > translation.Count())
                {
                    output -= translation.Count();
                }
            }
            else
            {
                output = valueInt - keyInt;

                if (output < 0)
                {
                    output += translation.Count();
                }
            }
            return output;
        }

        public static int Count<T>(this T @enum) where T : Enum
        {
            return Enum.GetValues(typeof(T)).Cast<T>().Count();
        }
    }

    internal enum Translation
    {
        A = 1,
        B = 2,
        C = 3,
        D = 4,
        E = 5,
        F = 6,
        G = 7,
        H = 8,
        I = 9,
        J = 10,
        K = 11,
        L = 12,
        M = 13,
        N = 14,
        O = 15,
        P = 16,
        Q = 17,
        R = 18,
        S = 19,
        T = 20,
        U = 21,
        V = 22,
        W = 23,
        X = 24,
        Y = 25,
        Z = 26
    }
}

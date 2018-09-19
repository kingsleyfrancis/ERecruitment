using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using ERecruitment.Core.Engine;
using Microsoft.Owin.Security.DataProtection;

namespace ERecruitment.Core.Common
{
    /// <summary>
    ///     Represents a common helper
    /// </summary>
    public static class CommonHelper
    {
        private static AspNetHostingPermissionLevel? _trustLevel;


        public static IDataProtectionProvider DataProtectionProvider { get; set; }

        public static bool OneToManyCollectionWrapperEnabled
        {
            get
            {
                bool enabled =
                    !String.IsNullOrEmpty(ConfigurationManager.AppSettings["OneToManyCollectionWrapperEnabled"]) &&
                    Convert.ToBoolean(ConfigurationManager
                        .AppSettings["OneToManyCollectionWrapperEnabled"]);
                return enabled;
            }
        }

        /// <summary>
        ///     Ensures the subscriber email is valid or throw.
        /// </summary>
        /// <param name="email">The email.</param>
        /// <returns></returns>
        public static string EnsureSubscriberEmailOrThrow(string email)
        {
            string output = EnsureNotNull(email);
            output = output.Trim();
            output = EnsureMaximumLength(output, 255);

            if (!IsValidEmail(output))
            {
                throw new Exception("Email is not valid.");
            }

            return output;
        }

        /// <summary>
        ///     Verifies that a string is in valid e-mail format
        /// </summary>
        /// <param name="email">Email to verify</param>
        /// <returns>true if the string is a valid e-mail address and false if it's not</returns>
        public static bool IsValidEmail(string email)
        {
            if (String.IsNullOrEmpty(email))
                return false;

            email = email.Trim();
            bool result = Regex.IsMatch(email,
                "^(?:[\\w\\!\\#\\$\\%\\&\\'\\*\\+\\-\\/\\=\\?\\^\\`\\{\\|\\}\\~]+\\.)*[\\w\\!\\#\\$\\%\\&\\'\\*\\+\\-\\/\\=\\?\\^\\`\\{\\|\\}\\~]+@(?:(?:(?:[a-zA-Z0-9](?:[a-zA-Z0-9\\-](?!\\.)){0,61}[a-zA-Z0-9]?\\.)+[a-zA-Z0-9](?:[a-zA-Z0-9\\-](?!$)){0,61}[a-zA-Z0-9]?)|(?:\\[(?:(?:[01]?\\d{1,2}|2[0-4]\\d|25[0-5])\\.){3}(?:[01]?\\d{1,2}|2[0-4]\\d|25[0-5])\\]))$",
                RegexOptions.IgnoreCase);
            return result;
        }

        /// <summary>
        ///     Generate random digit code
        /// </summary>
        /// <param name="length">Length to return</param>
        /// <param name="isSecured">Determines whether secure digit code is required.</param>
        /// <returns>Result string</returns>
        public static string GenerateRandomDigitCode(int length, bool isSecured = false)
        {
            string str = String.Empty;
            var random = new Random();

            for (int i = 0; i < length; i++)
            {
                str = isSecured
                    ? String.Concat(str, GenerateRandomInteger(0, 9))
                    : String.Concat(str, random.Next(0, 9));
            }
            return str;
        }

        /// <summary>
        ///     Returns an random integer number within a specified rage
        /// </summary>
        /// <param name="min">Minimum number</param>
        /// <param name="max">Maximum number</param>
        /// <returns>Result</returns>
        public static int GenerateRandomInteger(int min = 0, int max = 2147483647)
        {
            var randomNumberBuffer = new byte[10];
            new RNGCryptoServiceProvider().GetBytes(randomNumberBuffer);
            return new Random(BitConverter.ToInt32(randomNumberBuffer, 0)).Next(min, max);
        }

        /// <summary>
        ///     Ensure that a string doesn't exceed maximum allowed length
        /// </summary>
        /// <param name="str">Input string</param>
        /// <param name="maxLength">Maximum length</param>
        /// <param name="postfix">A string to add to the end if the original string was shorten</param>
        /// <returns>Input string if its lengh is OK; otherwise, truncated input string</returns>
        public static string EnsureMaximumLength(string str, int maxLength, string postfix = null)
        {
            if (String.IsNullOrEmpty(str))
                return str;

            if (str.Length > maxLength)
            {
                string result = str.Substring(0, maxLength);
                if (!String.IsNullOrEmpty(postfix))
                {
                    result += postfix;
                }
                return result;
            }
            return str;
        }

        /// <summary>
        ///     Ensures that a string only contains numeric values
        /// </summary>
        /// <param name="str">Input string</param>
        /// <returns>Input string with only numeric values, empty string if input is null/empty</returns>
        public static string EnsureNumericOnly(string str)
        {
            if (String.IsNullOrEmpty(str))
                return String.Empty;

            var result = new StringBuilder();
            foreach (char c in str)
            {
                if (Char.IsDigit(c))
                    result.Append(c);
            }
            return result.ToString();
        }


        public static bool IsWithinIntegerRange(int numbers)
        {
            return (numbers > 0 && numbers <= Int32.MaxValue);
        }

        public static bool IsNumerics(string str)
        {
            if (String.IsNullOrWhiteSpace(str))
                return false;

            str = str.Trim();

            return str.All(Char.IsDigit);
        }

        /// <summary>
        ///     Ensure that a string is not null
        /// </summary>
        /// <param name="str">Input string</param>
        /// <returns>Result</returns>
        public static string EnsureNotNull(string str)
        {
            if (str == null)
                return String.Empty;

            return str;
        }

        /// <summary>
        ///     Indicates whether the specified strings are null or empty strings
        /// </summary>
        /// <param name="stringsToValidate">Array of strings to validate</param>
        /// <returns>Boolean</returns>
        public static bool AreNullOrEmpty(params string[] stringsToValidate)
        {
            bool result = false;
            Array.ForEach(stringsToValidate, str => { if (String.IsNullOrEmpty(str)) result = true; });
            return result;
        }

        /// <summary>
        ///     Finds the trust level of the running application
        ///     (http://blogs.msdn.com/dmitryr/archive/2007/01/23/finding-out-the-current-trust-level-in-asp-net.aspx)
        /// </summary>
        /// <returns>The current trust level.</returns>
        public static AspNetHostingPermissionLevel GetTrustLevel()
        {
            if (!_trustLevel.HasValue)
            {
                //set minimum
                _trustLevel = AspNetHostingPermissionLevel.None;

                //determine maximum
                foreach (AspNetHostingPermissionLevel trustLevel in
                    new[]
                    {
                        AspNetHostingPermissionLevel.Unrestricted,
                        AspNetHostingPermissionLevel.High,
                        AspNetHostingPermissionLevel.Medium,
                        AspNetHostingPermissionLevel.Low,
                        AspNetHostingPermissionLevel.Minimal
                    })
                {
                    try
                    {
                        new AspNetHostingPermission(trustLevel).Demand();
                        _trustLevel = trustLevel;
                        break; //we've set the highest permission we can
                    }
                    catch (SecurityException)
                    {
                    }
                }
            }
            return _trustLevel.Value;
        }

        /// <summary>
        ///     Sets a property on an object to a valuae.
        /// </summary>
        /// <param name="instance">The object whose property to set.</param>
        /// <param name="propertyName">The name of the property to set.</param>
        /// <param name="value">The value to set the property to.</param>
        public static void SetProperty(object instance, string propertyName, object value)
        {
            if (instance == null) throw new ArgumentNullException("instance");
            if (propertyName == null) throw new ArgumentNullException("propertyName");

            Type instanceType = instance.GetType();
            PropertyInfo pi = instanceType.GetProperty(propertyName);

            if (pi == null)
            {
                string msg = string.Format("No property '{0}' found on the instance of type '{1}'.", propertyName,
                    instanceType);

                throw new Exception(msg);
            }

            if (!pi.CanWrite)
            {
                string msg = string.Format("The property '{0}' on the instance of type '{1}' does not have a setter.",
                    propertyName, instanceType);

                throw new Exception();
            }

            if (value != null && !value.GetType().IsAssignableFrom(pi.PropertyType))
                value = To(value, pi.PropertyType);
            pi.SetValue(instance, value, new object[0]);
        }

        public static TypeConverter GetNopCustomTypeConverter(Type type)
        {
            //we can't use the following code in order to register our custom type descriptors
            //TypeDescriptor.AddAttributes(typeof(List<int>),
            //new TypeConverterAttribute(typeof(GenericListTypeConverter<int>)));
            //so we do it manually here

            if (type == typeof (List<int>))
                return new GenericListTypeConverter<int>();
            if (type == typeof (List<decimal>))
                return new GenericListTypeConverter<decimal>();
            if (type == typeof (List<string>))
                return new GenericListTypeConverter<string>();

            return TypeDescriptor.GetConverter(type);
        }

        /// <summary>
        ///     Splits a string into a list of words that contains a specific character separator
        /// </summary>
        /// <param name="words">String to separated</param>
        /// <param name="separator">Character separator</param>
        /// <returns>List of separated words.</returns>
        public static List<string> Separate(string words, char separator)
        {
            if (String.IsNullOrWhiteSpace(words))
                return new List<string>();

            words = words.Trim();
            List<string> separated = words.Split(new[] {separator}).ToList();
            return separated;
        }

        /// <summary>
        ///     Converts a value to a destination type.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="destinationType">The type to convert the value to.</param>
        /// <returns>The converted value.</returns>
        public static object To(object value, Type destinationType)
        {
            return To(value, destinationType, CultureInfo.InvariantCulture);
        }

        /// <summary>
        ///     Converts a value to a destination type.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="destinationType">The type to convert the value to.</param>
        /// <param name="culture">Culture</param>
        /// <returns>The converted value.</returns>
        public static object To(object value, Type destinationType, CultureInfo culture)
        {
            if (value != null)
            {
                Type sourceType = value.GetType();

                TypeConverter destinationConverter = GetNopCustomTypeConverter(destinationType);

                TypeConverter sourceConverter = GetNopCustomTypeConverter(sourceType);

                if (destinationConverter != null && destinationConverter.CanConvertFrom(sourceType))
                    return destinationConverter.ConvertFrom(null, culture, value);
                if (sourceConverter != null && sourceConverter.CanConvertTo(destinationType))
                    return sourceConverter.ConvertTo(null, culture, value, destinationType);
                if (destinationType.IsEnum && value is int)
                    return Enum.ToObject(destinationType, (int) value);
                if (!destinationType.IsAssignableFrom(value.GetType()))
                    return Convert.ChangeType(value, destinationType, culture);
            }
            return value;
        }

        /// <summary>
        ///     Converts a value to a destination type.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <typeparam name="T">The type to convert the value to.</typeparam>
        /// <returns>The converted value.</returns>
        public static T To<T>(object value)
        {
            //return (T)Convert.ChangeType(value, typeof(T), CultureInfo.InvariantCulture);
            return (T) To(value, typeof (T));
        }

        /// <summary>
        ///     Convert enum for front-end
        /// </summary>
        /// <param name="str">Input string</param>
        /// <returns>Converted string</returns>
        public static string ConvertEnum(string str)
        {
            string result = String.Empty;
            char[] letters = str.ToCharArray();
            foreach (char c in letters)
                if (c.ToString() != c.ToString().ToLower())
                    result += " " + c;
                else
                    result += c.ToString();
            return result;
        }

        public static string FormatToViewable(string number, string separator, int separationCount)
        {
            if (String.IsNullOrWhiteSpace(number))
                return String.Empty;

            char[] numberArray = number.ToCharArray();

            if (numberArray.Length <= separationCount)
                return number;

            int startCount = 0;
            var result = new StringBuilder();
            int separatorAppendedCount = 1;
            int groupCount = numberArray.Length/separationCount;

            foreach (char num in numberArray)
            {
                if (startCount <= separationCount)
                {
                    result.Append(num);
                }
                startCount += 1;

                if (startCount != separationCount)
                    continue;

                if (separatorAppendedCount < groupCount)
                {
                    result.Append(separator);
                    separatorAppendedCount++;
                }
                startCount = 0;
            }
            return result.ToString();
        }

        /// <summary>
        ///     Sub divides strings into a group separated by a specified character
        /// </summary>
        /// <param name="wordToSeparate">String to separate</param>
        /// <param name="count">Count of characters in each group</param>
        /// <param name="separator">Character to separate with</param>
        /// <returns>Separated string</returns>
        public static string Separate(string wordToSeparate, int count, char separator)
        {
            if (String.IsNullOrWhiteSpace(wordToSeparate))
                return String.Empty;

            var separated = new StringBuilder();

            int currentCount = 0;
            foreach (char cha in wordToSeparate)
            {
                separated.Append(cha);
                if (currentCount == count)
                {
                    separated.Append(separator);
                    currentCount = 0;
                }
                else
                {
                    currentCount++;
                }
            }
            return separated.ToString();
        }

        /// <summary>
        ///     Replaces spaces in words with a specified character
        /// </summary>
        /// <param name="words">Words to replace</param>
        /// <param name="characterToReplaceWith">Replacement character (default: _)</param>
        /// <returns>Replaced words</returns>
        public static string ReplaceSpacesWith(string words, char characterToReplaceWith = '_')
        {
            if (string.IsNullOrWhiteSpace(words))
                return string.Empty;

            words = words.Trim();

            string[] splitted = words.Split(new[] {' ', ',', '.', ';', ':', '$', '#', '@'});

            string newString = splitted
                .Aggregate(string.Empty, (current, s) => current + (s + characterToReplaceWith));

            return newString;
        }


        /// <summary>
        ///     Compresses a given string by removing other characters that are not in the alphabet
        /// </summary>
        /// <param name="wordsToCompress"></param>
        /// <returns>Compressed strings.</returns>
        public static string Compress(string wordsToCompress)
        {
            if (string.IsNullOrWhiteSpace(wordsToCompress))
                return string.Empty;

            var sb = new StringBuilder();

            foreach (char c in wordsToCompress
                .Where(c => Char.IsLetter(c) || Char.IsDigit(c)))
            {
                sb.Append(c);
            }

            return sb.ToString();
        }

        /// <summary>
        ///     Mask the characters of the supplied words with X
        /// </summary>
        /// <param name="stringToMask">String to mask</param>
        /// <param name="leaveOut">How many characters to leave out</param>
        /// <returns>Masked string</returns>
        public static string Mask(string stringToMask, int leaveOut = 3)
        {
            if (string.IsNullOrWhiteSpace(stringToMask))
                return string.Empty;

            //Compress the string.
            string compressedString = Compress(stringToMask);
            int stringLength = compressedString.Count();

            if (stringLength > 0 && stringLength > leaveOut)
            {
                int startIndex = stringLength - leaveOut;

                //Take string that you dont want to mask
                string leftOut = compressedString.Substring(startIndex, leaveOut);

                List<string> repeated = Enumerable.Repeat("X", startIndex).ToList();

                string newString = string.Empty;
                repeated.ForEach(a => newString += a);

                newString += leftOut;

                //format to viewable
                string formatted = FormatToViewable(newString, "-", 4);
                return formatted;
            }
            else
            {
                List<string> repeated = Enumerable.Repeat("X", stringLength).ToList();
                string generatedString = string.Empty;

                repeated.ForEach(a => generatedString += a);

                //format to viewable
                string formatted = FormatToViewable(generatedString, "-", 4);
                return formatted;
            }
        }

        /// <summary>
        ///     Removes punctuation from the supplied text
        /// </summary>
        /// <param name="text"></param>
        /// <param name="replaceWith"></param>
        /// <returns></returns>
        public static string RemovePunctuations(string text, char replaceWith = '-')
        {
            if (string.IsNullOrWhiteSpace(text))
                return string.Empty;

            var punctuations = new[]
            {
                '!', '~', '@', '#', '$', '%', '^', '&', '*',
                '(', ')', ')', '+', '=', '<', '>', '?', '/', '|',
                '"', '"', ';', ':', '\'', '\\', ',', '.', ' '
            };
            text = text.Trim();
            string[] splitted = text.Split(new[] {' '});


            string result = splitted.Select(s => s.Trim(punctuations))
                .Aggregate(string.Empty,
                    (current, trimmed) => current + (trimmed + replaceWith));

            result = result.TrimEnd('-');

            return result;
        }

        public static Task LogFatalErrors(Exception exception, string msg = "")
        {
            var task = Task.Run(async () =>
            {
                const string path = "~/App_Data/Errors";
                var webHelper = EngineContext.Current.Resolve<IWebHelper>();

                if (exception == null &&
                    string.IsNullOrWhiteSpace(msg))
                    return;

                if (webHelper != null)
                {
                    string actualPath = webHelper.MapPath(path);
                    string fileName = string.Format("{0}.txt", DateTime.Now.Ticks);
                    string filePath = Path.Combine(actualPath, fileName);

                    if (exception != null)
                    {
                        string exceptionMsg = "<<Message>>\n",
                            stackTrace = "<<Stack Trace>>\n",
                            source = "<<Source>>\n",
                            targetSite = "<<Target Site>>",
                            additionInformation = "<<Additional Informations>>";
                        Exception exp = exception;
                        do
                        {
                            exceptionMsg += exp.Message;
                            stackTrace += exp.StackTrace;
                            source += exp.Source;
                            targetSite += exp.TargetSite;

                            IDictionary infos = exception.Data;
                            if (infos.Count > 0)
                            {
                                additionInformation = infos
                                    .Cast<object>()
                                    .Aggregate(additionInformation, (current, info) => current + info.ToString());
                            }
                            exp = exception.InnerException;
                        } while (exp != null);

                        var logs = new List<string>
                        {
                            exceptionMsg,
                            " ",
                            stackTrace,
                            " ",
                            source,
                            " ",
                            targetSite,
                            " ",
                            additionInformation,
                            " ",
                            msg
                        };
                        using (var file = new StreamWriter(filePath))
                        {
                            foreach (string log in logs)
                            {
                                await file.WriteLineAsync(exceptionMsg);
                            }
                        }
                    }
                    else
                    {
                        using (var file = new StreamWriter(filePath))
                        {
                            await file.WriteLineAsync(msg);
                        }
                    }
                }
            });
            return task;
        }
    }
}
/// <summary>
/// VL02 EX2
//
// Implement and write an algorithm!
//
// Iterate the following transformations with input data:
//
// Replace every 0 with 1 and replace every 1 with 10
// Example: 101 ⇒ 10110
// Use the newly created data for the next iteration
//
// Use 0 as initial input. What can be seen?
//
/// </summary>
using System;
using System.Diagnostics;
using System.Collections.Generic;

namespace vl02
{
	class MainClass
	{
		/// <summary>
		/// Print length and value of a string to console.
		/// </summary>
		/// <param name="str">String.</param>
		private static void PrintString(string str)
		{
			Console.WriteLine(str.Length.ToString() + " " + str);
		}

		/// <summary>
		/// Perform transformations
		/// </summary>
		/// <param name="count">Number of cycles to perform.</param>
		public static void DoTransformations1(int count)
		{
			string e = "0";
			PrintString (e);

			for(int k = 0; k < count; k++) {
				// Seems like string.Split will not work with empty separator.
				// This is a lengthy workaround.
				List<string> letters = new List<string> ();
				foreach (char c in e.ToCharArray()) {
					letters.Add (c.ToString());
				}

				// vv This was the original intention, which does not work.
				//var letters = e.Split (new String [] {""}, StringSplitOptions.None);

				for (int i = 0; i < letters.Count; i++) {
					switch (letters [i]) {
					case "0":
						letters [i] = "1";
						break;
					case "1":
						letters [i] = "10";
						break;
					}
				}

				e = string.Join ("", letters);
				PrintString (e);
			}
		}

		/// <summary>
		/// Slightly optimized version of the first implementation.
		/// </summary>
		/// <param name="count">Number of cycles to perform.</param>
		public static void DoTransformations2(int count)
		{
			string e = "0";
			PrintString (e);

			while(count-- > 0) {
				List<string> letters = new List<string> ();
				foreach(char c in e.ToCharArray()) {
					switch (c) {
					case '0':
						letters.Add("1");
						break;
					case '1':
						letters.Add("10");
						break;
					}
				}

				e = string.Join ("", letters);
				PrintString (e);
			}
		}

		/// <summary>
		/// A shorter version using string.Replace.
		/// 
		/// </summary>
		/// <param name="count">Number of cycles to perform.</param>
		public static void DoTransformations3(int count)
		{
			string e = "0";
			PrintString (e);

			while(count-- > 0) {
				e = e.Replace ('0', 'A').Replace ("1", "10").Replace ('A', '1');

				PrintString (e);
			}
		}

		/// <summary>
		/// A different version using string concatenation.
		/// </summary>
		/// <param name="count">Number of cycles to perform.</param>
		public static void DoTransformations4(int count)
		{
			string a = "0";
			string b = "1";

			PrintString (a);
			PrintString (b);

			count--;
			while (count-- > 0) {
				string c = b + a;
				PrintString (c);
				a = b;
				b = c;
			}
		}

		/// <summary>
		/// The entry point of the program, where the program control starts and ends.
		/// </summary>
		/// <param name="args">The command-line arguments.</param>
		public static void Main (string[] args)
		{
			int count = 10;

			DoTransformations1 (count);
			DoTransformations2 (count);
			DoTransformations3 (count);
			DoTransformations4 (count);
		}
	}
}

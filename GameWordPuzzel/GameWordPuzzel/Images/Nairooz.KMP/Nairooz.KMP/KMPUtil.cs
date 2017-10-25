// ==================================================================
// KMPUtil.cs
// ==================================================================
// © Copyright nairooz001@yahoo.com
// ==================================================================
// ================================================================================================ 
// Maintenance log - insert most recent change descriptions at top 
// 
// Date             Defect #           Who					  Description
//
// 13-Dec-08                          Nairooz NIlafdeen       Initial File Creation
// ================================================================================================
using System;
using System.Collections;

namespace Nairooz.KMP
{
	/// <summary>
	/// Summary description for KMPUtil.
	/// </summary>
	public sealed class KMPUtil
	{
		private KMPUtil(){}

		/// <summary>
		/// Finds all the occurences a pattern in a a string
		/// </summary>
		/// <param name="pattern">The pattern to search for</param>
		/// <param name="targetString">The target string to search for</param>
		/// <returns>
		/// Return an Arraylist containing the indexs where the 
		/// patternn occured
		/// </returns>
		public static ArrayList GetAllOccurences(string pattern, string targetString)
		{
			return GetOccurences(pattern, targetString);
		}

		/// <summary>
		/// Finds all the occurences a pattern in a string in reverse order
		/// </summary>
		/// <param name="pattern">The pattern to search for</param>
		/// <param name="targetString">
		/// The target string to search for. This string is actually reversed
		/// </param>
		/// <returns>
		/// Return an Arraylist containing the indexs where the 
		/// patternn occured
		/// </returns>
		public static ArrayList GetOccurencesForReverseString(string pattern, string targetString)
		{
            char[] array = pattern.ToCharArray();
			Array.Reverse(array);

			return GetOccurences(new string(array), targetString);			
		}

		/// <summary>
		/// Finds all the occurences a pattern in a a string
		/// </summary>
		/// <param name="pattern">The pattern to search for</param>
		/// <param name="targetString">The target string to search for</param>
		/// <returns>
		/// Return an Arraylist containing the indexs where the 
		/// patternn occured
		/// </returns>
		private static ArrayList GetOccurences(string pattern , string targetString)
		{
			ArrayList result;
			int[] transitionArray ;
			char[] charArray;
			char[] patternArray;

			charArray					= targetString.ToLower().ToCharArray();
			patternArray				= pattern.ToLower().ToCharArray();
			result					    = new ArrayList();

			PrefixArray  prefixArray	= new PrefixArray(pattern);
			transitionArray				= prefixArray.TransitionArray;
						
			//Keeps track of the pattern index
			int k = 0;

			for(int i = 0; i < charArray.Length; i++)
			{
				//If there is a match
				if(charArray[i] == patternArray[k])	
				{
					//Move the pattern index by one
					k++;
				}
				else
				{
					//There is a mismatch..so move the pattern 
					
					//The amount to move through the pattern	
					int prefix = transitionArray[k];

					//if the current char does not match
					//the char in the pattern that concides
					//when moved then shift the pattern entirley, so 
					//we dont make a unnecssary comparision
					if(prefix + 1 > patternArray.Length && 
						charArray[i] != patternArray[prefix + 1])
					{

						k = 0;
					}
					else
					{
						k = prefix;
					}
				}

				//A complet match, if kis
				//equal to pattern length
				if(k == patternArray.Length)
				{
					//Add it to our result
					result.Add(i - (patternArray.Length - 1));

					//Set k as if the next character is a mismatch
					//therefore we dont mis out any other containing
					//pattern
					k  = transitionArray[k - 1];
				}
			}	
	
			return result;
		}
	}
}

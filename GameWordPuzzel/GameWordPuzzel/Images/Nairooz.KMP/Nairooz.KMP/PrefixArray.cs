// ==================================================================
// Anagram.cs
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
using System.Collections.Specialized;

namespace Nairooz.KMP
{
	/// <summary>
	/// Summary description for PrefixArray.
	/// </summary>
	public class PrefixArray
	{
		/// <summary>
		/// The pattern to compute the 
		/// array
		/// </summary>
		private string pattern;

		private int[] hArray;

		/// <summary>
		/// Constructs a prefix array
		/// </summary>
		/// <param name="pattern">
		/// The to be used to construct
		/// the prefix array</param>
		public PrefixArray(string pattern)
		{
			if(pattern == null || pattern.Length == 0)
			{
				throw new ArgumentException
					("The pattern may not be null or 0 lenght", "pattern");
			}

			this.pattern	= pattern;			
			hArray			= new int[pattern.Length]; 
			ComputeHArray();
		}

		/// <summary>
		/// Computes the prefix array
		/// </summary>
		private void ComputeHArray()
		{
			/*Array to keep track of the sub string
			in each iteration*/
			char[] temp = null;
			
			//An array containing the characters of the string
			char[] patternArray = pattern.ToCharArray(); 

			//The first character in the string...
			//At this point the patern length is validated to be atleast 1
			char firstChar = patternArray[0];

			//This defaults to 0
			hArray[0] = 0;
			
			for(int i = 1 ; i < pattern.Length ; i++)
			{
				temp		= SubCharArray(i, patternArray);
		        hArray[i]	= GetPrefixLegth(temp, firstChar);    
			}
		}

		private static int GetPrefixLegth(char[] array, char charToMatch)
		{
			for(int i = 2; i < array.Length; i++)
			{
				//if it is a match
				if(array[i] == charToMatch)
				{
					if(IsSuffixExist(i, array))
					{
						//Return on the first prefix which is the largest
						return array.Length - i;
					}
				}
			}

			return 0;
		}

		/// <summary>
		/// Tests whether a suffix exists from the specified index
		/// </summary>
		/// <param name="index">
		/// The index of the char[] to start looking
		/// for the prefix
		///  </param>
		/// <param name="array">The source array</param>
		/// <returns>
		/// A bool; true if a prefix exist at the 
		/// specified pos</returns>
		private static bool IsSuffixExist(int index, char[] array)
		{			
			//Keep track of the prefix index
			int k = 0;
			for(int i = index; i < array.Length ; i++)
			{
				//A mismatch so return
				if(array[i] != array[k]){ return false;}		
				k++;
			}
			return true;
		}

		/// <summary>
		/// Creates a sub char[] from the source array 
		/// </summary>
		/// <param name="endIndex">
		/// The end index to until which 
		/// the copying should occur</param>
		/// <param name="array">The source array</param>
		/// <returns>A sub array</returns>
		private static char[] SubCharArray(int endIndex, char[] array)
		{
			char[] targetArray = new char[endIndex + 1];
			for(int i = 0 ; i <= endIndex ; i++)			
			{
				targetArray[i] = array[i];
			}
			return targetArray;			
		}

		/// <summary>
		/// Gets the transition array
		/// </summary>
		public int[] TransitionArray
		{
			get
			{
				return hArray;
			}
		}		
		
	}
}


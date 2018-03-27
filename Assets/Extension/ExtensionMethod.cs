using System.Collections.Generic;

public static class ExtensionMethod {

	//dictionary extension method

	/// <summary>
	/// Tries the get.
	/// </summary>
	/// <param name="dict">Dict.</param>
	/// <param name="key">Key.</param>
	/// <typeparam name="Tkey">The 1st type parameter.</typeparam>
	/// <typeparam name="Tvalue">The 2nd type parameter.</typeparam>
	public static Tvalue TryGet<Tkey, Tvalue>(this Dictionary<Tkey, Tvalue> dict, Tkey key)
	{
		Tvalue value;

		lock (dict) 
		{
			dict.TryGetValue (key, out value);
		}

		return value;
	}


}

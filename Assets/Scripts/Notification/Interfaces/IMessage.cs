/* 
 LidxFramework Code By kaikai
*/
using System;

public interface IMessage
{
	string Name { get; }

	object Body { get; set; }
		
	string Type { get; set; }

    string ToString();
}
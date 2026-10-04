// Squad.cs: Contains the class definition for a squad.
//
// A squad is a collection of units, and will appear on the WorldMap.



using System.Collections.Generic;
using UnityEngine;

public class Squad : MonoBehaviour
{
    public List<Unit> units { get; private set; } = new List<Unit>();
    public Team CurrentTeam { get; private set; }

    public void SetTeam(Team newTeam)
    {
        CurrentTeam = newTeam;
    }
}

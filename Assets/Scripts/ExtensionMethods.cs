﻿using System;
using UnityEngine;
using System.Collections;

public static class ExtensionMethods
{
    public static int GetTeam(this GameObject g)
    {
        var teamMember = g.transform.root.GetComponent<TeamMember>();
        return teamMember == null
            ? 0
            : teamMember.Team;
    }

    public static void SetTeam(this GameObject g, int team)
    {
        var teamMember = g.GetComponent<TeamMember>();
        if (teamMember == null)
            throw new Exception("Game object " + g + " does not have a TeamMember component to set");
        teamMember.Team = team;
    }
}
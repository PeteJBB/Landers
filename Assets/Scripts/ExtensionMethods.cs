﻿using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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

    public static Transform[] FindChildren(this Transform t, string name)
    {
        var children = new List<Transform>();
        for (var i = 0; i < t.childCount; i++)
        {
            var child = t.GetChild(i);
            if(child.name == name)
                children.Add(child);
        }
        return children.ToArray();
    }

    public static Vector3 IgnoreX(this Vector3 v, float newX = 0)
    {
        return new Vector3(newX, v.y, v.z);
    }
    
    public static Vector3 IgnoreY(this Vector3 v, float newY = 0)
    {
        return new Vector3(v.x, newY, v.z);
    }

    public static Vector3 IgnoreZ(this Vector3 v, float newZ = 0)
    {
        return new Vector3(v.x, v.y, newZ);
    }


}

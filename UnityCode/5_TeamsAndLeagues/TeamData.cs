using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "TeamData", menuName = "Football/Team Data")]
public class TeamData : ScriptableObject
{
    [Header("Team Info")]
    public string teamName;
    public int teamId;
    public string shortName;
    public string country;
    public string city;
    public int foundedYear;
    
    [Header("Team Visual")]
    public Sprite teamLogo;
    public Color primaryColor = Color.white;
    public Color secondaryColor = Color.black;
    public Color homeKitColor = Color.white;
    public Color awayKitColor = Color.red;
    
    [Header("Stadium")]
    public string stadiumName;
    public int stadiumCapacity;
    public Material stadiumMaterial;
    
    [Header("Team Formation")]
    public Formation formation = Formation.FourFourTwo;
    public List<PlayerData> players = new List<PlayerData>();
    public List<PlayerData> substitutes = new List<PlayerData>();
    
    [Header("Team Stats")]
    [Range(1, 99)]
    public int attack = 50;
    [Range(1, 99)]
    public int midfield = 50;
    [Range(1, 99)]
    public int defense = 50;
    [Range(1, 99)]
    public int overall = 50;
    
    [Header("Team Finances")]
    public int budget = 1000000;
    public int transferBudget = 500000;
    public int wageBudget = 100000;
    
    [Header("Team History")]
    public int leagueTitles = 0;
    public int cupTitles = 0;
    public int internationalTitles = 0;
    
    void OnValidate()
    {
        CalculateTeamRating();
        ValidateSquad();
    }
    
    public void CalculateTeamRating()
    {
        if (players.Count == 0) return;
        
        int totalAttack = 0;
        int totalMidfield = 0;
        int totalDefense = 0;
        int totalOverall = 0;
        
        foreach (PlayerData player in players)
        {
            if (player != null)
            {
                // Calculate positional ratings
                switch (player.preferredPosition)
                {
                    case PlayerPosition.Striker:
                    case PlayerPosition.LeftWing:
                    case PlayerPosition.RightWing:
                    case PlayerPosition.AttackingMidfield:
                        totalAttack += player.overall;
                        break;
                    case PlayerPosition.CentralMidfield:
                    case PlayerPosition.DefensiveMidfield:
                        totalMidfield += player.overall;
                        break;
                    case PlayerPosition.CenterBack:
                    case PlayerPosition.LeftBack:
                    case PlayerPosition.RightBack:
                    case PlayerPosition.Goalkeeper:
                        totalDefense += player.overall;
                        break;
                }
                
                totalOverall += player.overall;
            }
        }
        
        // Calculate averages
        attack = Mathf.RoundToInt(totalAttack / (float)GetPlayerCountByPosition(PlayerPosition.Striker, PlayerPosition.AttackingMidfield, PlayerPosition.LeftWing, PlayerPosition.RightWing));
        midfield = Mathf.RoundToInt(totalMidfield / (float)GetPlayerCountByPosition(PlayerPosition.CentralMidfield, PlayerPosition.DefensiveMidfield));
        defense = Mathf.RoundToInt(totalDefense / (float)GetPlayerCountByPosition(PlayerPosition.CenterBack, PlayerPosition.LeftBack, PlayerPosition.RightBack, PlayerPosition.Goalkeeper));
        overall = Mathf.RoundToInt(totalOverall / (float)players.Count);
        
        // Clamp values
        attack = Mathf.Clamp(attack, 1, 99);
        midfield = Mathf.Clamp(midfield, 1, 99);
        defense = Mathf.Clamp(defense, 1, 99);
        overall = Mathf.Clamp(overall, 1, 99);
    }
    
    int GetPlayerCountByPosition(params PlayerPosition[] positions)
    {
        int count = 0;
        foreach (PlayerData player in players)
        {
            if (player != null)
            {
                foreach (PlayerPosition pos in positions)
                {
                    if (player.preferredPosition == pos)
                    {
                        count++;
                        break;
                    }
                }
            }
        }
        return Mathf.Max(1, count); // Avoid division by zero
    }
    
    public void ValidateSquad()
    {
        // Ensure we have enough players
        if (players.Count < 11)
        {
            Debug.LogWarning($"Team {teamName} has less than 11 players!");
        }
        
        // Ensure we have a goalkeeper
        bool hasGoalkeeper = false;
        foreach (PlayerData player in players)
        {
            if (player != null && player.preferredPosition == PlayerPosition.Goalkeeper)
            {
                hasGoalkeeper = true;
                break;
            }
        }
        
        if (!hasGoalkeeper)
        {
            Debug.LogWarning($"Team {teamName} has no goalkeeper!");
        }
    }
    
    public List<PlayerData> GetStartingEleven()
    {
        List<PlayerData> startingEleven = new List<PlayerData>();
        
        // Add goalkeeper
        foreach (PlayerData player in players)
        {
            if (player != null && player.preferredPosition == PlayerPosition.Goalkeeper)
            {
                startingEleven.Add(player);
                break;
            }
        }
        
        // Add field players based on formation
        switch (formation)
        {
            case Formation.FourFourTwo:
                AddPlayersByPosition(startingEleven, PlayerPosition.CenterBack, 2);
                AddPlayersByPosition(startingEleven, PlayerPosition.LeftBack, 1);
                AddPlayersByPosition(startingEleven, PlayerPosition.RightBack, 1);
                AddPlayersByPosition(startingEleven, PlayerPosition.CentralMidfield, 4);
                AddPlayersByPosition(startingEleven, PlayerPosition.Striker, 2);
                break;
            case Formation.FourThreeThree:
                AddPlayersByPosition(startingEleven, PlayerPosition.CenterBack, 2);
                AddPlayersByPosition(startingEleven, PlayerPosition.LeftBack, 1);
                AddPlayersByPosition(startingEleven, PlayerPosition.RightBack, 1);
                AddPlayersByPosition(startingEleven, PlayerPosition.CentralMidfield, 3);
                AddPlayersByPosition(startingEleven, PlayerPosition.LeftWing, 1);
                AddPlayersByPosition(startingEleven, PlayerPosition.RightWing, 1);
                AddPlayersByPosition(startingEleven, PlayerPosition.Striker, 1);
                break;
            case Formation.ThreeFiveTwo:
                AddPlayersByPosition(startingEleven, PlayerPosition.CenterBack, 3);
                AddPlayersByPosition(startingEleven, PlayerPosition.CentralMidfield, 3);
                AddPlayersByPosition(startingEleven, PlayerPosition.LeftWing, 1);
                AddPlayersByPosition(startingEleven, PlayerPosition.RightWing, 1);
                AddPlayersByPosition(startingEleven, PlayerPosition.Striker, 2);
                break;
        }
        
        return startingEleven;
    }
    
    void AddPlayersByPosition(List<PlayerData> startingEleven, PlayerPosition position, int count)
    {
        int added = 0;
        foreach (PlayerData player in players)
        {
            if (player != null && player.preferredPosition == position && added < count)
            {
                startingEleven.Add(player);
                added++;
            }
        }
    }
    
    public PlayerData GetBestPlayer()
    {
        PlayerData bestPlayer = null;
        int highestRating = 0;
        
        foreach (PlayerData player in players)
        {
            if (player != null && player.overall > highestRating)
            {
                highestRating = player.overall;
                bestPlayer = player;
            }
        }
        
        return bestPlayer;
    }
    
    public List<PlayerData> GetPlayersByPosition(PlayerPosition position)
    {
        List<PlayerData> positionPlayers = new List<PlayerData>();
        
        foreach (PlayerData player in players)
        {
            if (player != null && player.preferredPosition == position)
            {
                positionPlayers.Add(player);
            }
        }
        
        return positionPlayers;
    }
    
    public bool CanAfford(int cost)
    {
        return budget >= cost;
    }
    
    public void SpendMoney(int amount)
    {
        budget -= amount;
        budget = Mathf.Max(0, budget);
    }
    
    public void EarnMoney(int amount)
    {
        budget += amount;
    }
}

public enum Formation
{
    FourFourTwo,
    FourThreeThree,
    ThreeFiveTwo,
    FourTwoThreeOne,
    FourThreeOneTwo,
    ThreeFourThree,
    FiveFourOne,
    FiveThreeTwo
}
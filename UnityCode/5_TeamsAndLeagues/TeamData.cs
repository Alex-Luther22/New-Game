using UnityEngine;

[CreateAssetMenu(fileName = "New Team", menuName = "Football Game/Team Data")]
public class TeamData : ScriptableObject
{
    [Header("Basic Team Info")]
    public string teamName;
    public int teamId;
    public string shortName; // Ej: "FCB", "RMA"
    public string city;
    public string country;
    public int foundedYear;
    
    [Header("Visual Identity")]
    public Sprite teamLogo;
    public Sprite teamBadge;
    public Color primaryColor = Color.white;
    public Color secondaryColor = Color.black;
    public Color accentColor = Color.blue;
    
    [Header("Kit Design")]
    public Material homeKitMaterial;
    public Material awayKitMaterial;
    public Material thirdKitMaterial;
    public Material goalkeeperKitMaterial;
    
    [Header("Stadium")]
    public string stadiumName;
    public int stadiumCapacity;
    public GameObject stadiumPrefab;
    
    [Header("Team Ratings")]
    [Range(1, 99)]
    public int overallRating = 75;
    
    [Range(1, 99)]
    public int attackRating = 75;
    
    [Range(1, 99)]
    public int midfieldRating = 75;
    
    [Range(1, 99)]
    public int defenseRating = 75;
    
    [Range(1, 99)]
    public int goalkeeperRating = 75;
    
    [Header("Playing Style")]
    public PlayingStyle preferredStyle;
    public FormationType defaultFormation;
    public int aggressiveness = 50; // 0-100
    public int possession = 50; // Preferencia por posesión vs contraataque
    public int tempo = 50; // Velocidad de juego
    
    [Header("Squad")]
    public PlayerData[] players = new PlayerData[23]; // Squad típico
    
    [Header("Manager")]
    public string managerName;
    public Sprite managerPhoto;
    public int managerRating = 75;
    
    [Header("Club Statistics")]
    public int leagueTitles = 0;
    public int cupTitles = 0;
    public int internationalTitles = 0;
    public int currentLeaguePosition = 1;
    
    [Header("Financial")]
    public long transferBudget = 50000000;
    public long wageBudget = 10000000;
    public long clubValue = 100000000;
    
    [Header("Rivalry")]
    public TeamData[] rivals;
    
    // Propiedades calculadas
    public int averageAge
    {
        get
        {
            int totalAge = 0;
            int playerCount = 0;
            
            foreach (PlayerData player in players)
            {
                if (player != null)
                {
                    totalAge += player.age;
                    playerCount++;
                }
            }
            
            return playerCount > 0 ? totalAge / playerCount : 0;
        }
    }
    
    public long totalSquadValue
    {
        get
        {
            long totalValue = 0;
            
            foreach (PlayerData player in players)
            {
                if (player != null)
                {
                    totalValue += player.marketValue;
                }
            }
            
            return totalValue;
        }
    }
    
    public int squadSize
    {
        get
        {
            int count = 0;
            
            foreach (PlayerData player in players)
            {
                if (player != null)
                {
                    count++;
                }
            }
            
            return count;
        }
    }
    
    // Métodos para obtener jugadores por posición
    public PlayerData[] GetPlayersByPosition(PlayerPosition position)
    {
        System.Collections.Generic.List<PlayerData> positionPlayers = new System.Collections.Generic.List<PlayerData>();
        
        foreach (PlayerData player in players)
        {
            if (player != null && player.preferredPosition == position)
            {
                positionPlayers.Add(player);
            }
        }
        
        return positionPlayers.ToArray();
    }
    
    // Obtener el mejor 11 inicial
    public PlayerData[] GetStartingEleven()
    {
        PlayerData[] startingEleven = new PlayerData[11];
        
        // Portero
        PlayerData[] goalkeepers = GetPlayersByPosition(PlayerPosition.Goalkeeper);
        if (goalkeepers.Length > 0)
        {
            startingEleven[0] = GetBestPlayer(goalkeepers);
        }
        
        // Según la formación por defecto
        switch (defaultFormation)
        {
            case FormationType.Formation_4_4_2:
                Fill442Formation(startingEleven);
                break;
            case FormationType.Formation_4_3_3:
                Fill433Formation(startingEleven);
                break;
            case FormationType.Formation_3_5_2:
                Fill352Formation(startingEleven);
                break;
            case FormationType.Formation_4_2_3_1:
                Fill4231Formation(startingEleven);
                break;
            case FormationType.Formation_4_1_4_1:
                Fill4141Formation(startingEleven);
                break;
        }
        
        return startingEleven;
    }
    
    void Fill442Formation(PlayerData[] eleven)
    {
        // Defensores (4)
        PlayerData[] defenders = GetPlayersByPosition(PlayerPosition.CenterBack);
        PlayerData[] leftBacks = GetPlayersByPosition(PlayerPosition.LeftBack);
        PlayerData[] rightBacks = GetPlayersByPosition(PlayerPosition.RightBack);
        
        eleven[1] = GetBestPlayer(leftBacks);
        eleven[2] = GetBestPlayer(defenders);
        eleven[3] = GetBestPlayer(defenders, eleven[2]);
        eleven[4] = GetBestPlayer(rightBacks);
        
        // Mediocampistas (4)
        PlayerData[] midfielders = GetPlayersByPosition(PlayerPosition.CentralMidfield);
        PlayerData[] wingers = GetPlayersByPosition(PlayerPosition.LeftWing);
        PlayerData[] rightWingers = GetPlayersByPosition(PlayerPosition.RightWing);
        
        eleven[5] = GetBestPlayer(wingers);
        eleven[6] = GetBestPlayer(midfielders);
        eleven[7] = GetBestPlayer(midfielders, eleven[6]);
        eleven[8] = GetBestPlayer(rightWingers);
        
        // Delanteros (2)
        PlayerData[] strikers = GetPlayersByPosition(PlayerPosition.Striker);
        eleven[9] = GetBestPlayer(strikers);
        eleven[10] = GetBestPlayer(strikers, eleven[9]);
    }
    
    void Fill433Formation(PlayerData[] eleven)
    {
        // Defensores (4)
        PlayerData[] defenders = GetPlayersByPosition(PlayerPosition.CenterBack);
        PlayerData[] leftBacks = GetPlayersByPosition(PlayerPosition.LeftBack);
        PlayerData[] rightBacks = GetPlayersByPosition(PlayerPosition.RightBack);
        
        eleven[1] = GetBestPlayer(leftBacks);
        eleven[2] = GetBestPlayer(defenders);
        eleven[3] = GetBestPlayer(defenders, eleven[2]);
        eleven[4] = GetBestPlayer(rightBacks);
        
        // Mediocampistas (3)
        PlayerData[] midfielders = GetPlayersByPosition(PlayerPosition.CentralMidfield);
        PlayerData[] dmfs = GetPlayersByPosition(PlayerPosition.DefensiveMidfield);
        
        eleven[5] = GetBestPlayer(dmfs);
        eleven[6] = GetBestPlayer(midfielders);
        eleven[7] = GetBestPlayer(midfielders, eleven[6]);
        
        // Delanteros (3)
        PlayerData[] wingers = GetPlayersByPosition(PlayerPosition.LeftWing);
        PlayerData[] rightWingers = GetPlayersByPosition(PlayerPosition.RightWing);
        PlayerData[] strikers = GetPlayersByPosition(PlayerPosition.Striker);
        
        eleven[8] = GetBestPlayer(wingers);
        eleven[9] = GetBestPlayer(strikers);
        eleven[10] = GetBestPlayer(rightWingers);
    }
    
    void Fill352Formation(PlayerData[] eleven)
    {
        // Defensores (3)
        PlayerData[] defenders = GetPlayersByPosition(PlayerPosition.CenterBack);
        
        eleven[1] = GetBestPlayer(defenders);
        eleven[2] = GetBestPlayer(defenders, eleven[1]);
        eleven[3] = GetBestPlayer(defenders, eleven[1], eleven[2]);
        
        // Mediocampistas (5)
        PlayerData[] leftBacks = GetPlayersByPosition(PlayerPosition.LeftBack);
        PlayerData[] rightBacks = GetPlayersByPosition(PlayerPosition.RightBack);
        PlayerData[] midfielders = GetPlayersByPosition(PlayerPosition.CentralMidfield);
        PlayerData[] dmfs = GetPlayersByPosition(PlayerPosition.DefensiveMidfield);
        
        eleven[4] = GetBestPlayer(leftBacks);
        eleven[5] = GetBestPlayer(dmfs);
        eleven[6] = GetBestPlayer(midfielders);
        eleven[7] = GetBestPlayer(midfielders, eleven[6]);
        eleven[8] = GetBestPlayer(rightBacks);
        
        // Delanteros (2)
        PlayerData[] strikers = GetPlayersByPosition(PlayerPosition.Striker);
        eleven[9] = GetBestPlayer(strikers);
        eleven[10] = GetBestPlayer(strikers, eleven[9]);
    }
    
    void Fill4231Formation(PlayerData[] eleven)
    {
        // Defensores (4)
        PlayerData[] defenders = GetPlayersByPosition(PlayerPosition.CenterBack);
        PlayerData[] leftBacks = GetPlayersByPosition(PlayerPosition.LeftBack);
        PlayerData[] rightBacks = GetPlayersByPosition(PlayerPosition.RightBack);
        
        eleven[1] = GetBestPlayer(leftBacks);
        eleven[2] = GetBestPlayer(defenders);
        eleven[3] = GetBestPlayer(defenders, eleven[2]);
        eleven[4] = GetBestPlayer(rightBacks);
        
        // Mediocampistas defensivos (2)
        PlayerData[] dmfs = GetPlayersByPosition(PlayerPosition.DefensiveMidfield);
        PlayerData[] midfielders = GetPlayersByPosition(PlayerPosition.CentralMidfield);
        
        eleven[5] = GetBestPlayer(dmfs);
        eleven[6] = GetBestPlayer(midfielders);
        
        // Mediocampistas ofensivos (3)
        PlayerData[] wingers = GetPlayersByPosition(PlayerPosition.LeftWing);
        PlayerData[] rightWingers = GetPlayersByPosition(PlayerPosition.RightWing);
        PlayerData[] amfs = GetPlayersByPosition(PlayerPosition.AttackingMidfield);
        
        eleven[7] = GetBestPlayer(wingers);
        eleven[8] = GetBestPlayer(amfs);
        eleven[9] = GetBestPlayer(rightWingers);
        
        // Delantero (1)
        PlayerData[] strikers = GetPlayersByPosition(PlayerPosition.Striker);
        eleven[10] = GetBestPlayer(strikers);
    }
    
    void Fill4141Formation(PlayerData[] eleven)
    {
        // Similar al 4-2-3-1 pero con diferentes roles
        Fill4231Formation(eleven);
    }
    
    // Obtener el mejor jugador de un array, excluyendo jugadores ya seleccionados
    PlayerData GetBestPlayer(PlayerData[] players, params PlayerData[] exclude)
    {
        PlayerData bestPlayer = null;
        int bestRating = 0;
        
        foreach (PlayerData player in players)
        {
            if (player != null && !System.Array.Exists(exclude, p => p == player))
            {
                if (player.overall > bestRating)
                {
                    bestRating = player.overall;
                    bestPlayer = player;
                }
            }
        }
        
        return bestPlayer;
    }
    
    // Método para generar un equipo aleatorio
    public static TeamData GenerateRandomTeam(string name, int id, string country)
    {
        TeamData team = ScriptableObject.CreateInstance<TeamData>();
        
        team.teamName = name;
        team.teamId = id;
        team.shortName = name.Substring(0, Mathf.Min(3, name.Length)).ToUpper();
        team.country = country;
        team.foundedYear = Random.Range(1880, 2010);
        
        // Generar ratings aleatorios
        int baseRating = Random.Range(65, 85);
        team.overallRating = baseRating;
        team.attackRating = baseRating + Random.Range(-5, 5);
        team.midfieldRating = baseRating + Random.Range(-5, 5);
        team.defenseRating = baseRating + Random.Range(-5, 5);
        team.goalkeeperRating = baseRating + Random.Range(-5, 5);
        
        // Generar estilo de juego aleatorio
        team.preferredStyle = (PlayingStyle)Random.Range(0, System.Enum.GetValues(typeof(PlayingStyle)).Length);
        team.defaultFormation = (FormationType)Random.Range(0, System.Enum.GetValues(typeof(FormationType)).Length);
        team.aggressiveness = Random.Range(30, 80);
        team.possession = Random.Range(30, 80);
        team.tempo = Random.Range(30, 80);
        
        // Generar colores aleatorios
        team.primaryColor = new Color(Random.value, Random.value, Random.value);
        team.secondaryColor = new Color(Random.value, Random.value, Random.value);
        team.accentColor = new Color(Random.value, Random.value, Random.value);
        
        // Generar estadio
        team.stadiumName = $"Estadio {name}";
        team.stadiumCapacity = Random.Range(20000, 80000);
        
        // Generar finanzas
        team.transferBudget = Random.Range(10000000, 200000000);
        team.wageBudget = Random.Range(5000000, 50000000);
        team.clubValue = Random.Range(50000000, 500000000);
        
        // Generar manager
        team.managerName = GenerateRandomManagerName();
        team.managerRating = Random.Range(60, 90);
        
        // Generar jugadores
        GenerateRandomSquad(team);
        
        return team;
    }
    
    static string GenerateRandomManagerName()
    {
        string[] firstNames = { "José", "Carlos", "Antonio", "Luis", "Miguel", "Pedro", "Juan", "Francisco", "Diego", "Roberto" };
        string[] lastNames = { "García", "Rodríguez", "González", "Fernández", "López", "Martínez", "Sánchez", "Pérez", "Gómez", "Martín" };
        
        return firstNames[Random.Range(0, firstNames.Length)] + " " + lastNames[Random.Range(0, lastNames.Length)];
    }
    
    static void GenerateRandomSquad(TeamData team)
    {
        string[] playerNames = { "Alejandro", "David", "Sergio", "Andrés", "Pablo", "Javier", "Manuel", "Raúl", "Iker", "Fernando", "Álvaro", "Marcos", "Rubén", "Adrián", "Gonzalo", "Víctor", "Dani", "Juanjo", "Koke", "Nacho" };
        
        int playerIndex = 0;
        
        // Generar porteros (3)
        for (int i = 0; i < 3; i++)
        {
            string name = playerNames[Random.Range(0, playerNames.Length)] + " " + Random.Range(1, 99);
            team.players[playerIndex] = PlayerData.GenerateRandomPlayer(name, team.teamId, PlayerPosition.Goalkeeper);
            playerIndex++;
        }
        
        // Generar defensores (8)
        PlayerPosition[] defenderPositions = { PlayerPosition.CenterBack, PlayerPosition.CenterBack, PlayerPosition.CenterBack, PlayerPosition.LeftBack, PlayerPosition.LeftBack, PlayerPosition.RightBack, PlayerPosition.RightBack, PlayerPosition.CenterBack };
        for (int i = 0; i < 8; i++)
        {
            string name = playerNames[Random.Range(0, playerNames.Length)] + " " + Random.Range(1, 99);
            team.players[playerIndex] = PlayerData.GenerateRandomPlayer(name, team.teamId, defenderPositions[i]);
            playerIndex++;
        }
        
        // Generar mediocampistas (8)
        PlayerPosition[] midfielderPositions = { PlayerPosition.DefensiveMidfield, PlayerPosition.CentralMidfield, PlayerPosition.CentralMidfield, PlayerPosition.AttackingMidfield, PlayerPosition.LeftWing, PlayerPosition.RightWing, PlayerPosition.CentralMidfield, PlayerPosition.DefensiveMidfield };
        for (int i = 0; i < 8; i++)
        {
            string name = playerNames[Random.Range(0, playerNames.Length)] + " " + Random.Range(1, 99);
            team.players[playerIndex] = PlayerData.GenerateRandomPlayer(name, team.teamId, midfielderPositions[i]);
            playerIndex++;
        }
        
        // Generar delanteros (4)
        for (int i = 0; i < 4; i++)
        {
            string name = playerNames[Random.Range(0, playerNames.Length)] + " " + Random.Range(1, 99);
            team.players[playerIndex] = PlayerData.GenerateRandomPlayer(name, team.teamId, PlayerPosition.Striker);
            playerIndex++;
        }
    }
}

public enum PlayingStyle
{
    Tiki_Taka,
    Counter_Attack,
    Direct_Play,
    Possession_Based,
    High_Press,
    Defensive,
    Wing_Play,
    Long_Ball
}

public enum FormationType
{
    Formation_4_4_2,
    Formation_4_3_3,
    Formation_3_5_2,
    Formation_4_2_3_1,
    Formation_4_1_4_1,
    Formation_3_4_3,
    Formation_5_3_2,
    Formation_4_5_1
}
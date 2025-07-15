from motor.motor_asyncio import AsyncIOMotorClient
from typing import List, Optional, Dict, Any
import os
from models import *

class DatabaseManager:
    def __init__(self):
        self.client = AsyncIOMotorClient(os.environ['MONGO_URL'])
        self.db = self.client[os.environ['DB_NAME']]
        
    async def initialize_data(self):
        """Initialize database with default teams, stadiums, and achievements"""
        await self.create_default_teams()
        await self.create_default_stadiums()
        await self.create_default_achievements()
        
    async def create_default_teams(self):
        """Create 100+ teams from major leagues without copyright issues"""
        default_teams = [
            # English Premier League (Top 6)
            {
                "name": "London Red", "short_name": "LRD", "country": "England", "league": "Premier League",
                "overall_rating": 88, "attack_rating": 90, "midfield_rating": 87, "defense_rating": 86,
                "primary_color": "#FF0000", "secondary_color": "#FFFFFF", "stadium_name": "Emirates Arena",
                "stadium_capacity": 60000, "budget": 200000000, "prestige": 9
            },
            {
                "name": "Manchester Blue", "short_name": "MCB", "country": "England", "league": "Premier League",
                "overall_rating": 91, "attack_rating": 93, "midfield_rating": 90, "defense_rating": 89,
                "primary_color": "#6CABDD", "secondary_color": "#FFFFFF", "stadium_name": "Etihad Stadium",
                "stadium_capacity": 55000, "budget": 250000000, "prestige": 10
            },
            {
                "name": "Manchester Red", "short_name": "MRD", "country": "England", "league": "Premier League",
                "overall_rating": 85, "attack_rating": 87, "midfield_rating": 84, "defense_rating": 83,
                "primary_color": "#FF0000", "secondary_color": "#FFFFFF", "stadium_name": "Old Trafford",
                "stadium_capacity": 75000, "budget": 180000000, "prestige": 9
            },
            {
                "name": "London Blue", "short_name": "LBL", "country": "England", "league": "Premier League",
                "overall_rating": 87, "attack_rating": 89, "midfield_rating": 86, "defense_rating": 85,
                "primary_color": "#034694", "secondary_color": "#FFFFFF", "stadium_name": "Stamford Bridge",
                "stadium_capacity": 42000, "budget": 220000000, "prestige": 8
            },
            {
                "name": "North London", "short_name": "NLD", "country": "England", "league": "Premier League",
                "overall_rating": 84, "attack_rating": 86, "midfield_rating": 83, "defense_rating": 82,
                "primary_color": "#132257", "secondary_color": "#FFFFFF", "stadium_name": "New White Hart Lane",
                "stadium_capacity": 62000, "budget": 150000000, "prestige": 7
            },
            {
                "name": "Liverpool Red", "short_name": "LIV", "country": "England", "league": "Premier League",
                "overall_rating": 89, "attack_rating": 91, "midfield_rating": 88, "defense_rating": 87,
                "primary_color": "#C8102E", "secondary_color": "#FFFFFF", "stadium_name": "Anfield",
                "stadium_capacity": 54000, "budget": 190000000, "prestige": 9
            },
            
            # Spanish La Liga
            {
                "name": "Madrid White", "short_name": "MDW", "country": "Spain", "league": "La Liga",
                "overall_rating": 92, "attack_rating": 94, "midfield_rating": 91, "defense_rating": 90,
                "primary_color": "#FFFFFF", "secondary_color": "#FFD700", "stadium_name": "Santiago Bernabéu",
                "stadium_capacity": 81000, "budget": 300000000, "prestige": 10
            },
            {
                "name": "Barcelona FC", "short_name": "BAR", "country": "Spain", "league": "La Liga",
                "overall_rating": 90, "attack_rating": 92, "midfield_rating": 89, "defense_rating": 88,
                "primary_color": "#A50044", "secondary_color": "#004D98", "stadium_name": "Camp Nou",
                "stadium_capacity": 99000, "budget": 280000000, "prestige": 10
            },
            {
                "name": "Madrid Red", "short_name": "ATM", "country": "Spain", "league": "La Liga",
                "overall_rating": 86, "attack_rating": 84, "midfield_rating": 87, "defense_rating": 89,
                "primary_color": "#CE2029", "secondary_color": "#FFFFFF", "stadium_name": "Wanda Metropolitano",
                "stadium_capacity": 70000, "budget": 160000000, "prestige": 8
            },
            {
                "name": "Sevilla FC", "short_name": "SEV", "country": "Spain", "league": "La Liga",
                "overall_rating": 82, "attack_rating": 80, "midfield_rating": 83, "defense_rating": 84,
                "primary_color": "#FFFFFF", "secondary_color": "#D2001F", "stadium_name": "Ramón Sánchez Pizjuán",
                "stadium_capacity": 43000, "budget": 80000000, "prestige": 7
            },
            
            # German Bundesliga
            {
                "name": "Bayern Munich", "short_name": "BAY", "country": "Germany", "league": "Bundesliga",
                "overall_rating": 90, "attack_rating": 92, "midfield_rating": 89, "defense_rating": 88,
                "primary_color": "#FF0000", "secondary_color": "#FFFFFF", "stadium_name": "Allianz Arena",
                "stadium_capacity": 75000, "budget": 250000000, "prestige": 9
            },
            {
                "name": "Borussia Dortmund", "short_name": "BVB", "country": "Germany", "league": "Bundesliga",
                "overall_rating": 85, "attack_rating": 87, "midfield_rating": 84, "defense_rating": 83,
                "primary_color": "#FDE100", "secondary_color": "#000000", "stadium_name": "Signal Iduna Park",
                "stadium_capacity": 81000, "budget": 140000000, "prestige": 8
            },
            
            # Italian Serie A
            {
                "name": "Juventus FC", "short_name": "JUV", "country": "Italy", "league": "Serie A",
                "overall_rating": 84, "attack_rating": 82, "midfield_rating": 85, "defense_rating": 86,
                "primary_color": "#FFFFFF", "secondary_color": "#000000", "stadium_name": "Allianz Stadium",
                "stadium_capacity": 41000, "budget": 120000000, "prestige": 8
            },
            {
                "name": "Milan AC", "short_name": "MIL", "country": "Italy", "league": "Serie A",
                "overall_rating": 83, "attack_rating": 85, "midfield_rating": 82, "defense_rating": 81,
                "primary_color": "#FF0000", "secondary_color": "#000000", "stadium_name": "San Siro",
                "stadium_capacity": 80000, "budget": 110000000, "prestige": 8
            },
            {
                "name": "Inter Milan", "short_name": "INT", "country": "Italy", "league": "Serie A",
                "overall_rating": 85, "attack_rating": 87, "midfield_rating": 84, "defense_rating": 83,
                "primary_color": "#0068A8", "secondary_color": "#000000", "stadium_name": "San Siro",
                "stadium_capacity": 80000, "budget": 130000000, "prestige": 8
            },
            
            # French Ligue 1
            {
                "name": "Paris Saint-Germain", "short_name": "PSG", "country": "France", "league": "Ligue 1",
                "overall_rating": 88, "attack_rating": 91, "midfield_rating": 86, "defense_rating": 85,
                "primary_color": "#1A4B84", "secondary_color": "#FF0000", "stadium_name": "Parc des Princes",
                "stadium_capacity": 48000, "budget": 400000000, "prestige": 8
            },
            {
                "name": "Marseille", "short_name": "MAR", "country": "France", "league": "Ligue 1",
                "overall_rating": 78, "attack_rating": 76, "midfield_rating": 79, "defense_rating": 80,
                "primary_color": "#0084C4", "secondary_color": "#FFFFFF", "stadium_name": "Vélodrome",
                "stadium_capacity": 67000, "budget": 60000000, "prestige": 6
            },
            
            # Brazilian League
            {
                "name": "Flamengo", "short_name": "FLA", "country": "Brazil", "league": "Brasileirão",
                "overall_rating": 82, "attack_rating": 85, "midfield_rating": 80, "defense_rating": 79,
                "primary_color": "#FF0000", "secondary_color": "#000000", "stadium_name": "Maracanã",
                "stadium_capacity": 78000, "budget": 40000000, "prestige": 7
            },
            {
                "name": "Santos FC", "short_name": "SAN", "country": "Brazil", "league": "Brasileirão",
                "overall_rating": 78, "attack_rating": 80, "midfield_rating": 77, "defense_rating": 76,
                "primary_color": "#FFFFFF", "secondary_color": "#000000", "stadium_name": "Vila Belmiro",
                "stadium_capacity": 20000, "budget": 25000000, "prestige": 7
            },
            
            # Argentine League
            {
                "name": "River Plate", "short_name": "RIV", "country": "Argentina", "league": "Primera División",
                "overall_rating": 80, "attack_rating": 82, "midfield_rating": 79, "defense_rating": 78,
                "primary_color": "#FFFFFF", "secondary_color": "#FF0000", "stadium_name": "Monumental",
                "stadium_capacity": 70000, "budget": 35000000, "prestige": 7
            },
            {
                "name": "Boca Juniors", "short_name": "BOC", "country": "Argentina", "league": "Primera División",
                "overall_rating": 81, "attack_rating": 83, "midfield_rating": 80, "defense_rating": 79,
                "primary_color": "#1E3A8A", "secondary_color": "#FFD700", "stadium_name": "La Bombonera",
                "stadium_capacity": 54000, "budget": 30000000, "prestige": 7
            }
        ]
        
        for team_data in default_teams:
            team = Team(**team_data)
            # Generate players for each team
            team.players = self.generate_team_players(team)
            await self.db.teams.insert_one(team.dict())
            
    def generate_team_players(self, team: Team) -> List[Player]:
        """Generate realistic players for a team"""
        import random
        
        players = []
        positions_needed = [
            (Position.GOALKEEPER, 3),
            (Position.DEFENDER, 8),
            (Position.MIDFIELDER, 8),
            (Position.FORWARD, 6)
        ]
        
        player_names = {
            Position.GOALKEEPER: ["Martinez", "Silva", "Johnson", "Mueller", "Garcia", "Lopez", "Brown", "Wilson"],
            Position.DEFENDER: ["Smith", "Jones", "Williams", "Davis", "Miller", "Anderson", "Taylor", "Thomas", "Jackson", "White", "Harris", "Martin", "Thompson", "Clark", "Rodriguez", "Lewis"],
            Position.MIDFIELDER: ["Robinson", "Walker", "Young", "Allen", "King", "Wright", "Scott", "Torres", "Nguyen", "Hill", "Flores", "Green", "Adams", "Nelson", "Baker", "Hall"],
            Position.FORWARD: ["Roberts", "Carter", "Mitchell", "Perez", "Turner", "Phillips", "Campbell", "Parker", "Evans", "Edwards", "Collins", "Stewart", "Sanchez", "Morris", "Rogers", "Reed"]
        }
        
        countries = ["England", "Spain", "Germany", "Italy", "France", "Brazil", "Argentina", "Portugal", "Netherlands", "Belgium", "Croatia", "Colombia", "Mexico", "Chile", "Uruguay"]
        
        for position, count in positions_needed:
            for i in range(count):
                base_rating = max(60, team.overall_rating - random.randint(5, 15))
                if i == 0 and position == Position.GOALKEEPER:  # Main goalkeeper
                    base_rating = team.overall_rating - random.randint(0, 5)
                elif i < 2:  # Starters
                    base_rating = team.overall_rating - random.randint(0, 8)
                
                player = Player(
                    name=random.choice(player_names[position]),
                    position=position,
                    overall_rating=max(50, min(99, base_rating)),
                    pace=random.randint(40, 95),
                    shooting=random.randint(30, 95),
                    passing=random.randint(40, 95),
                    defending=random.randint(30, 95),
                    physicality=random.randint(40, 95),
                    age=random.randint(18, 35),
                    nationality=random.choice(countries),
                    value=random.randint(100000, 50000000),
                    stamina=random.randint(65, 95),
                    skill_moves=random.randint(1, 5),
                    weak_foot=random.randint(1, 5)
                )
                players.append(player)
        
        return players
    
    async def create_default_stadiums(self):
        """Create unique stadiums with special characteristics"""
        stadiums = [
            {
                "name": "Majestic Arena", "capacity": 80000, "country": "England", "city": "London",
                "surface_type": "hybrid_grass", "roof_type": "retractable", "atmosphere_rating": 9,
                "weather_conditions": ["rainy", "cloudy", "sunny"],
                "unique_features": ["historic_stands", "royal_box", "steep_stands"]
            },
            {
                "name": "Cathedral of Football", "capacity": 99000, "country": "Spain", "city": "Barcelona",
                "surface_type": "natural_grass", "roof_type": "open", "atmosphere_rating": 10,
                "weather_conditions": ["sunny", "windy"],
                "unique_features": ["massive_capacity", "steep_stands", "iconic_architecture"]
            },
            {
                "name": "Bernabéu Stadium", "capacity": 81000, "country": "Spain", "city": "Madrid",
                "surface_type": "hybrid_grass", "roof_type": "retractable", "atmosphere_rating": 9,
                "weather_conditions": ["sunny", "hot"],
                "unique_features": ["modern_design", "royal_box", "premium_facilities"]
            },
            {
                "name": "Allianz Arena", "capacity": 75000, "country": "Germany", "city": "Munich",
                "surface_type": "natural_grass", "roof_type": "closed", "atmosphere_rating": 9,
                "weather_conditions": ["cold", "snow", "sunny"],
                "unique_features": ["color_changing_exterior", "modern_design", "excellent_acoustics"]
            },
            {
                "name": "Signal Iduna Park", "capacity": 81000, "country": "Germany", "city": "Dortmund",
                "surface_type": "natural_grass", "roof_type": "partial", "atmosphere_rating": 10,
                "weather_conditions": ["cold", "cloudy", "sunny"],
                "unique_features": ["yellow_wall", "amazing_atmosphere", "standing_section"]
            },
            {
                "name": "San Siro", "capacity": 80000, "country": "Italy", "city": "Milan",
                "surface_type": "natural_grass", "roof_type": "open", "atmosphere_rating": 9,
                "weather_conditions": ["mild", "foggy", "sunny"],
                "unique_features": ["historic_stadium", "shared_venue", "iconic_architecture"]
            },
            {
                "name": "Parc des Princes", "capacity": 48000, "country": "France", "city": "Paris",
                "surface_type": "hybrid_grass", "roof_type": "open", "atmosphere_rating": 8,
                "weather_conditions": ["mild", "rainy", "sunny"],
                "unique_features": ["intimate_atmosphere", "modern_facilities", "city_stadium"]
            },
            {
                "name": "Maracanã", "capacity": 78000, "country": "Brazil", "city": "Rio de Janeiro",
                "surface_type": "natural_grass", "roof_type": "partial", "atmosphere_rating": 10,
                "weather_conditions": ["hot", "humid", "sunny"],
                "unique_features": ["legendary_stadium", "world_cup_venue", "samba_atmosphere"]
            },
            {
                "name": "La Bombonera", "capacity": 54000, "country": "Argentina", "city": "Buenos Aires",
                "surface_type": "natural_grass", "roof_type": "open", "atmosphere_rating": 10,
                "weather_conditions": ["mild", "windy", "sunny"],
                "unique_features": ["steep_stands", "intimidating_atmosphere", "unique_shape"]
            },
            {
                "name": "Anfield", "capacity": 54000, "country": "England", "city": "Liverpool",
                "surface_type": "natural_grass", "roof_type": "open", "atmosphere_rating": 9,
                "weather_conditions": ["rainy", "windy", "cloudy"],
                "unique_features": ["the_kop", "amazing_atmosphere", "historic_stadium"]
            }
        ]
        
        for stadium_data in stadiums:
            stadium = Stadium(**stadium_data)
            await self.db.stadiums.insert_one(stadium.dict())
    
    async def create_default_achievements(self):
        """Create comprehensive achievement system"""
        achievements = [
            # Match Achievements
            {
                "name": "First Goal", "description": "Score your first goal",
                "icon": "⚽", "category": "Scoring", "reward_xp": 100, "reward_coins": 500,
                "rarity": "common", "unlock_condition": "goals_scored >= 1",
                "requirement": {"goals_scored": 1}
            },
            {
                "name": "Hat-trick Hero", "description": "Score 3 goals in a single match",
                "icon": "🎩", "category": "Scoring", "reward_xp": 500, "reward_coins": 2000,
                "rarity": "rare", "unlock_condition": "goals_in_match >= 3",
                "requirement": {"goals_in_match": 3}
            },
            {
                "name": "Clean Sheet", "description": "Win a match without conceding",
                "icon": "🥅", "category": "Defending", "reward_xp": 200, "reward_coins": 1000,
                "rarity": "common", "unlock_condition": "clean_sheets >= 1",
                "requirement": {"clean_sheets": 1}
            },
            {
                "name": "Unbeaten Run", "description": "Go 10 matches without losing",
                "icon": "🔥", "category": "Streak", "reward_xp": 1000, "reward_coins": 5000,
                "rarity": "epic", "unlock_condition": "unbeaten_streak >= 10",
                "requirement": {"unbeaten_streak": 10}
            },
            {
                "name": "Perfect Season", "description": "Win all matches in a season",
                "icon": "👑", "category": "Season", "reward_xp": 5000, "reward_coins": 25000,
                "rarity": "legendary", "unlock_condition": "perfect_season == true",
                "requirement": {"perfect_season": True}
            },
            
            # Career Achievements
            {
                "name": "New Manager", "description": "Start your first career",
                "icon": "👔", "category": "Career", "reward_xp": 200, "reward_coins": 1000,
                "rarity": "common", "unlock_condition": "career_started == true",
                "requirement": {"career_started": True}
            },
            {
                "name": "Trophy Hunter", "description": "Win your first trophy",
                "icon": "🏆", "category": "Trophies", "reward_xp": 800, "reward_coins": 3000,
                "rarity": "rare", "unlock_condition": "trophies_won >= 1",
                "requirement": {"trophies_won": 1}
            },
            {
                "name": "Transfer Master", "description": "Complete 50 transfers",
                "icon": "🔄", "category": "Transfers", "reward_xp": 1500, "reward_coins": 7500,
                "rarity": "epic", "unlock_condition": "transfers_completed >= 50",
                "requirement": {"transfers_completed": 50}
            },
            
            # Skill Achievements
            {
                "name": "Skill Master", "description": "Successfully perform 100 skill moves",
                "icon": "🌟", "category": "Skills", "reward_xp": 1000, "reward_coins": 4000,
                "rarity": "rare", "unlock_condition": "skill_moves_successful >= 100",
                "requirement": {"skill_moves_successful": 100}
            },
            {
                "name": "Rainbow Master", "description": "Perform 10 Rainbow Flicks",
                "icon": "🌈", "category": "Skills", "reward_xp": 500, "reward_coins": 2000,
                "rarity": "rare", "unlock_condition": "rainbow_flicks >= 10",
                "requirement": {"rainbow_flicks": 10}
            },
            
            # Special Achievements
            {
                "name": "World Traveler", "description": "Play in 20 different stadiums",
                "icon": "🌍", "category": "Exploration", "reward_xp": 2000, "reward_coins": 10000,
                "rarity": "epic", "unlock_condition": "stadiums_played >= 20",
                "requirement": {"stadiums_played": 20}
            },
            {
                "name": "Legend", "description": "Reach level 50",
                "icon": "⭐", "category": "Progress", "reward_xp": 10000, "reward_coins": 50000,
                "rarity": "legendary", "unlock_condition": "level >= 50",
                "requirement": {"level": 50}
            }
        ]
        
        for achievement_data in achievements:
            achievement = Achievement(**achievement_data)
            await self.db.achievements.insert_one(achievement.dict())
    
    # CRUD Operations
    async def create_user_profile(self, profile_data: dict) -> str:
        profile = UserProfile(**profile_data)
        result = await self.db.user_profiles.insert_one(profile.dict())
        return str(result.inserted_id)
    
    async def get_user_profile(self, user_id: str) -> Optional[UserProfile]:
        result = await self.db.user_profiles.find_one({"id": user_id})
        return UserProfile(**result) if result else None
    
    async def get_all_teams(self) -> List[Team]:
        cursor = self.db.teams.find()
        teams = []
        async for team_doc in cursor:
            teams.append(Team(**team_doc))
        return teams
    
    async def get_team_by_id(self, team_id: str) -> Optional[Team]:
        result = await self.db.teams.find_one({"id": team_id})
        return Team(**result) if result else None
    
    async def get_teams_by_league(self, league: str) -> List[Team]:
        cursor = self.db.teams.find({"league": league})
        teams = []
        async for team_doc in cursor:
            teams.append(Team(**team_doc))
        return teams
    
    async def get_all_stadiums(self) -> List[Stadium]:
        cursor = self.db.stadiums.find()
        stadiums = []
        async for stadium_doc in cursor:
            stadiums.append(Stadium(**stadium_doc))
        return stadiums
    
    async def create_match(self, match_data: dict) -> str:
        match = Match(**match_data)
        result = await self.db.matches.insert_one(match.dict())
        return str(result.inserted_id)
    
    async def get_user_matches(self, user_id: str) -> List[Match]:
        cursor = self.db.matches.find({"player_id": user_id})
        matches = []
        async for match_doc in cursor:
            matches.append(Match(**match_doc))
        return matches
    
    async def create_tournament(self, tournament_data: dict) -> str:
        tournament = Tournament(**tournament_data)
        result = await self.db.tournaments.insert_one(tournament.dict())
        return str(result.inserted_id)
    
    async def get_all_achievements(self) -> List[Achievement]:
        cursor = self.db.achievements.find()
        achievements = []
        async for achievement_doc in cursor:
            achievements.append(Achievement(**achievement_doc))
        return achievements
    
    async def update_user_achievement(self, user_id: str, achievement_id: str):
        await self.db.user_profiles.update_one(
            {"id": user_id},
            {"$addToSet": {"achievements": achievement_id}}
        )
    
    async def create_career(self, career_data: dict) -> str:
        career = Career(**career_data)
        result = await self.db.careers.insert_one(career.dict())
        return str(result.inserted_id)
    
    async def get_user_career(self, user_id: str) -> Optional[Career]:
        result = await self.db.careers.find_one({"user_id": user_id})
        return Career(**result) if result else None
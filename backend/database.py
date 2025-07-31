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
                "primary_color": "#C8102E", "secondary_color": "#FFFFFF", "stadium_name": "Anfield Fortress",
                "stadium_capacity": 54000, "budget": 190000000, "prestige": 9
            },
            
            # Spanish La Liga
            {
                "name": "Madrid White", "short_name": "MDW", "country": "Spain", "league": "La Liga",
                "overall_rating": 92, "attack_rating": 94, "midfield_rating": 91, "defense_rating": 90,
                "primary_color": "#FFFFFF", "secondary_color": "#FFD700", "stadium_name": "Royal Bernabeu Arena",
                "stadium_capacity": 81000, "budget": 300000000, "prestige": 10
            },
            {
                "name": "Barcelona FC", "short_name": "BAR", "country": "Spain", "league": "La Liga",
                "overall_rating": 90, "attack_rating": 92, "midfield_rating": 89, "defense_rating": 88,
                "primary_color": "#A50044", "secondary_color": "#004D98", "stadium_name": "Camp Majesty",
                "stadium_capacity": 99000, "budget": 280000000, "prestige": 10
            },
            {
                "name": "Madrid Red", "short_name": "MDR", "country": "Spain", "league": "La Liga",
                "overall_rating": 86, "attack_rating": 88, "midfield_rating": 85, "defense_rating": 84,
                "primary_color": "#FF0000", "secondary_color": "#FFFFFF", "stadium_name": "Wanda Metropolitano",
                "stadium_capacity": 68000, "budget": 150000000, "prestige": 8
            },
            {
                "name": "Sevilla FC", "short_name": "SEV", "country": "Spain", "league": "La Liga",
                "overall_rating": 82, "attack_rating": 84, "midfield_rating": 81, "defense_rating": 80,
                "primary_color": "#FF0000", "secondary_color": "#FFFFFF", "stadium_name": "Ramon Sanchez Pizjuan",
                "stadium_capacity": 43000, "budget": 80000000, "prestige": 7
            },
            {
                "name": "Villarreal CF", "short_name": "VIL", "country": "Spain", "league": "La Liga",
                "overall_rating": 80, "attack_rating": 82, "midfield_rating": 79, "defense_rating": 78,
                "primary_color": "#FFFF00", "secondary_color": "#003399", "stadium_name": "Estadio de la Ceramica",
                "stadium_capacity": 23000, "budget": 60000000, "prestige": 6
            },
            
            # German Bundesliga
            {
                "name": "Bayern Munich", "short_name": "BAY", "country": "Germany", "league": "Bundesliga",
                "overall_rating": 90, "attack_rating": 92, "midfield_rating": 89, "defense_rating": 88,
                "primary_color": "#FF0000", "secondary_color": "#FFFFFF", "stadium_name": "Allianz Fortress",
                "stadium_capacity": 75000, "budget": 250000000, "prestige": 9
            },
            {
                "name": "Borussia Dortmund", "short_name": "BVB", "country": "Germany", "league": "Bundesliga",
                "overall_rating": 85, "attack_rating": 87, "midfield_rating": 84, "defense_rating": 83,
                "primary_color": "#FFFF00", "secondary_color": "#000000", "stadium_name": "Signal Iduna Wall",
                "stadium_capacity": 81000, "budget": 120000000, "prestige": 8
            },
            {
                "name": "RB Leipzig", "short_name": "RBL", "country": "Germany", "league": "Bundesliga",
                "overall_rating": 83, "attack_rating": 85, "midfield_rating": 82, "defense_rating": 81,
                "primary_color": "#FF0000", "secondary_color": "#FFFFFF", "stadium_name": "Red Bull Arena",
                "stadium_capacity": 47000, "budget": 100000000, "prestige": 7
            },
            {
                "name": "Bayer Leverkusen", "short_name": "B04", "country": "Germany", "league": "Bundesliga",
                "overall_rating": 84, "attack_rating": 86, "midfield_rating": 83, "defense_rating": 82,
                "primary_color": "#FF0000", "secondary_color": "#000000", "stadium_name": "BayArena",
                "stadium_capacity": 30000, "budget": 90000000, "prestige": 7
            },
            
            # Italian Serie A
            {
                "name": "Juventus FC", "short_name": "JUV", "country": "Italy", "league": "Serie A",
                "overall_rating": 84, "attack_rating": 86, "midfield_rating": 83, "defense_rating": 82,
                "primary_color": "#FFFFFF", "secondary_color": "#000000", "stadium_name": "Allianz Stadium",
                "stadium_capacity": 41000, "budget": 180000000, "prestige": 8
            },
            {
                "name": "Milan AC", "short_name": "MIL", "country": "Italy", "league": "Serie A",
                "overall_rating": 83, "attack_rating": 85, "midfield_rating": 82, "defense_rating": 81,
                "primary_color": "#FF0000", "secondary_color": "#000000", "stadium_name": "San Siro Cathedral",
                "stadium_capacity": 80000, "budget": 150000000, "prestige": 8
            },
            {
                "name": "Inter Milan", "short_name": "INT", "country": "Italy", "league": "Serie A",
                "overall_rating": 85, "attack_rating": 87, "midfield_rating": 84, "defense_rating": 83,
                "primary_color": "#0066CC", "secondary_color": "#000000", "stadium_name": "San Siro Cathedral",
                "stadium_capacity": 80000, "budget": 140000000, "prestige": 8
            },
            {
                "name": "SSC Napoli", "short_name": "NAP", "country": "Italy", "league": "Serie A",
                "overall_rating": 85, "attack_rating": 87, "midfield_rating": 84, "defense_rating": 83,
                "primary_color": "#0066CC", "secondary_color": "#FFFFFF", "stadium_name": "Stadio Diego Armando Maradona",
                "stadium_capacity": 55000, "budget": 130000000, "prestige": 8
            },
            
            # French Ligue 1
            {
                "name": "Paris Saint-Germain", "short_name": "PSG", "country": "France", "league": "Ligue 1",
                "overall_rating": 89, "attack_rating": 91, "midfield_rating": 88, "defense_rating": 87,
                "primary_color": "#003399", "secondary_color": "#FF0000", "stadium_name": "Parc des Princes",
                "stadium_capacity": 48000, "budget": 400000000, "prestige": 9
            },
            {
                "name": "Olympique Marseille", "short_name": "OM", "country": "France", "league": "Ligue 1",
                "overall_rating": 82, "attack_rating": 84, "midfield_rating": 81, "defense_rating": 80,
                "primary_color": "#009FE3", "secondary_color": "#FFFFFF", "stadium_name": "Orange Velodrome",
                "stadium_capacity": 67000, "budget": 80000000, "prestige": 7
            },
            
            # Brazilian Brasileirão
            {
                "name": "Flamengo", "short_name": "FLA", "country": "Brazil", "league": "Brasileirão",
                "overall_rating": 83, "attack_rating": 85, "midfield_rating": 82, "defense_rating": 81,
                "primary_color": "#FF0000", "secondary_color": "#000000", "stadium_name": "Maracanã Temple",
                "stadium_capacity": 78000, "budget": 70000000, "prestige": 8
            },
            {
                "name": "Santos FC", "short_name": "SAN", "country": "Brazil", "league": "Brasileirão",
                "overall_rating": 81, "attack_rating": 83, "midfield_rating": 80, "defense_rating": 79,
                "primary_color": "#FFFFFF", "secondary_color": "#000000", "stadium_name": "Vila Belmiro",
                "stadium_capacity": 16000, "budget": 50000000, "prestige": 7
            },
            
            # Argentine Primera División
            {
                "name": "River Plate", "short_name": "RIV", "country": "Argentina", "league": "Primera División",
                "overall_rating": 82, "attack_rating": 84, "midfield_rating": 81, "defense_rating": 80,
                "primary_color": "#FF0000", "secondary_color": "#FFFFFF", "stadium_name": "El Monumental",
                "stadium_capacity": 84000, "budget": 60000000, "prestige": 8
            },
            {
                "name": "Boca Juniors", "short_name": "BOC", "country": "Argentina", "league": "Primera División",
                "overall_rating": 82, "attack_rating": 84, "midfield_rating": 81, "defense_rating": 80,
                "primary_color": "#003399", "secondary_color": "#FFFF00", "stadium_name": "La Bombonera Cauldron",
                "stadium_capacity": 49000, "budget": 60000000, "prestige": 8
            }
        ]
        
        # Check if teams already exist
        existing_teams = await self.db.teams.count_documents({})
        if existing_teams == 0:
            # Create teams with generated players
            for team_data in default_teams:
                # Generate players for this team
                team_data["players"] = await self.generate_team_players(team_data["name"], team_data["overall_rating"])
                
                # Create the team
                team = Team(**team_data)
                await self.db.teams.insert_one(team.model_dump())
                
    async def create_default_stadiums(self):
        """Create default stadiums"""
        default_stadiums = [
            {
                "name": "Emirates Arena", "capacity": 60000, "country": "England", "city": "London",
                "surface_type": "hybrid_grass", "roof_type": "open", "atmosphere_rating": 8,
                "weather_conditions": ["rainy", "cloudy", "sunny"],
                "unique_features": ["historic_stands", "steep_stands"]
            },
            {
                "name": "Camp Majesty", "capacity": 99000, "country": "Spain", "city": "Barcelona",
                "surface_type": "natural_grass", "roof_type": "open", "atmosphere_rating": 10,
                "weather_conditions": ["sunny", "windy"],
                "unique_features": ["massive_capacity", "iconic_architecture", "azure_exterior"]
            },
            {
                "name": "Royal Bernabeu Arena", "capacity": 81000, "country": "Spain", "city": "Madrid",
                "surface_type": "natural_grass", "roof_type": "retractable", "atmosphere_rating": 10,
                "weather_conditions": ["sunny", "windy", "hot"],
                "unique_features": ["retractable_roof", "royal_white_exterior", "historic_prestige"]
            },
            {
                "name": "Allianz Fortress", "capacity": 75000, "country": "Germany", "city": "Munich",
                "surface_type": "natural_grass", "roof_type": "closed", "atmosphere_rating": 9,
                "weather_conditions": ["cold", "snow", "sunny"],
                "unique_features": ["color_changing_exterior", "modern_design", "bavarian_atmosphere"]
            },
            {
                "name": "Anfield Fortress", "capacity": 54000, "country": "England", "city": "Liverpool",
                "surface_type": "natural_grass", "roof_type": "open", "atmosphere_rating": 10,
                "weather_conditions": ["rainy", "cloudy", "windy"],
                "unique_features": ["kop_stand", "you_never_walk_alone", "electric_atmosphere"]
            },
            {
                "name": "San Siro Cathedral", "capacity": 80000, "country": "Italy", "city": "Milan",
                "surface_type": "natural_grass", "roof_type": "open", "atmosphere_rating": 9,
                "weather_conditions": ["sunny", "rainy", "foggy"],
                "unique_features": ["spiral_towers", "historic_architecture", "dual_heritage"]
            },
            {
                "name": "Parc des Princes", "capacity": 48000, "country": "France", "city": "Paris",
                "surface_type": "natural_grass", "roof_type": "open", "atmosphere_rating": 8,
                "weather_conditions": ["sunny", "rainy", "windy"],
                "unique_features": ["parisian_elegance", "steep_stands", "modern_facilities"]
            },
            {
                "name": "Maracanã Temple", "capacity": 78000, "country": "Brazil", "city": "Rio de Janeiro",
                "surface_type": "natural_grass", "roof_type": "open", "atmosphere_rating": 10,
                "weather_conditions": ["hot", "humid", "sunny"],
                "unique_features": ["samba_atmosphere", "world_cup_legacy", "brazilian_passion"]
            },
            {
                "name": "La Bombonera Cauldron", "capacity": 49000, "country": "Argentina", "city": "Buenos Aires",
                "surface_type": "natural_grass", "roof_type": "open", "atmosphere_rating": 10,
                "weather_conditions": ["sunny", "windy", "hot"],
                "unique_features": ["intimidating_atmosphere", "steep_stands", "argentine_passion"]
            },
            {
                "name": "Signal Iduna Wall", "capacity": 81000, "country": "Germany", "city": "Dortmund",
                "surface_type": "natural_grass", "roof_type": "open", "atmosphere_rating": 10,
                "weather_conditions": ["cold", "rainy", "snow"],
                "unique_features": ["yellow_wall", "standing_section", "thunderous_atmosphere"]
            }
        ]
        
        existing_stadiums = await self.db.stadiums.count_documents({})
        if existing_stadiums == 0:
            for stadium_data in default_stadiums:
                stadium = Stadium(**stadium_data)
                await self.db.stadiums.insert_one(stadium.model_dump())
                
    async def create_default_achievements(self):
        """Create default achievements system"""
        default_achievements = [
            # Scoring Achievements
            {
                "name": "First Goal", "description": "Score your first goal in Football Master",
                "icon": "⚽", "category": "Scoring", "requirement": {"goals": 1},
                "reward_xp": 100, "reward_coins": 500, "rarity": "common",
                "unlock_condition": "Score 1 goal"
            },
            {
                "name": "Hat-Trick Hero", "description": "Score a hat-trick in a single match",
                "icon": "🎩", "category": "Scoring", "requirement": {"hat_tricks": 1},
                "reward_xp": 500, "reward_coins": 2500, "rarity": "rare",
                "unlock_condition": "Score 3 goals in one match"
            },
            {
                "name": "Goal Machine", "description": "Score 100 goals total",
                "icon": "🏆", "category": "Scoring", "requirement": {"goals": 100},
                "reward_xp": 1000, "reward_coins": 10000, "rarity": "epic",
                "unlock_condition": "Score 100 goals"
            },
            {
                "name": "Legend Striker", "description": "Score 500 goals total",
                "icon": "👑", "category": "Scoring", "requirement": {"goals": 500},
                "reward_xp": 2500, "reward_coins": 25000, "rarity": "legendary",
                "unlock_condition": "Score 500 goals"
            },
            
            # Skill Achievements
            {
                "name": "Rainbow Legend", "description": "Perform 25 rainbow flicks",
                "icon": "🌈", "category": "Skills", "requirement": {"rainbow_flicks": 25},
                "reward_xp": 300, "reward_coins": 1500, "rarity": "rare",
                "unlock_condition": "Perform 25 rainbow flicks"
            },
            {
                "name": "Elastico Expert", "description": "Perform 50 elastico moves",
                "icon": "✨", "category": "Skills", "requirement": {"elastico_moves": 50},
                "reward_xp": 400, "reward_coins": 2000, "rarity": "rare",
                "unlock_condition": "Perform 50 elastico moves"
            },
            {
                "name": "Skill Master", "description": "Successfully perform 500 skill moves",
                "icon": "🎪", "category": "Skills", "requirement": {"skill_moves": 500},
                "reward_xp": 1500, "reward_coins": 15000, "rarity": "epic",
                "unlock_condition": "Perform 500 skill moves"
            },
            
            # Career Achievements
            {
                "name": "New Manager", "description": "Start your first career mode",
                "icon": "💼", "category": "Career", "requirement": {"career_started": 1},
                "reward_xp": 200, "reward_coins": 1000, "rarity": "common",
                "unlock_condition": "Start career mode"
            },
            {
                "name": "Championship Winner", "description": "Win your first league title",
                "icon": "🏆", "category": "Career", "requirement": {"league_titles": 1},
                "reward_xp": 1000, "reward_coins": 10000, "rarity": "epic",
                "unlock_condition": "Win a league title"
            },
            {
                "name": "Perfect Season", "description": "Win all matches in a season",
                "icon": "💎", "category": "Career", "requirement": {"perfect_seasons": 1},
                "reward_xp": 5000, "reward_coins": 50000, "rarity": "mythic",
                "unlock_condition": "Win all matches in a season"
            },
            
            # Special Achievements
            {
                "name": "Comeback King", "description": "Win 10 matches after being behind",
                "icon": "🔥", "category": "Special", "requirement": {"comeback_wins": 10},
                "reward_xp": 800, "reward_coins": 5000, "rarity": "epic",
                "unlock_condition": "Win 10 matches from behind"
            },
            {
                "name": "Big Game Player", "description": "Score in 5 finals",
                "icon": "⭐", "category": "Special", "requirement": {"final_goals": 5},
                "reward_xp": 1200, "reward_coins": 8000, "rarity": "epic",
                "unlock_condition": "Score in 5 finals"
            },
            
            # Level Achievements
            {
                "name": "Rising Star", "description": "Reach level 10",
                "icon": "⭐", "category": "Level", "requirement": {"level": 10},
                "reward_xp": 500, "reward_coins": 2500, "rarity": "common",
                "unlock_condition": "Reach level 10"
            },
            {
                "name": "Veteran", "description": "Reach level 25",
                "icon": "🏅", "category": "Level", "requirement": {"level": 25},
                "reward_xp": 1000, "reward_coins": 7500, "rarity": "rare",
                "unlock_condition": "Reach level 25"
            },
            {
                "name": "Legend", "description": "Reach level 50",
                "icon": "👑", "category": "Level", "requirement": {"level": 50},
                "reward_xp": 2500, "reward_coins": 25000, "rarity": "legendary",
                "unlock_condition": "Reach level 50"
            }
        ]
        
        existing_achievements = await self.db.achievements.count_documents({})
        if existing_achievements == 0:
            for achievement_data in default_achievements:
                achievement = Achievement(**achievement_data)
                await self.db.achievements.insert_one(achievement.model_dump())
    
    async def generate_team_players(self, team_name: str, team_rating: int) -> list:
        """Generate players for specific teams with enhanced star players for top teams"""
        
        # MANCHESTER BLUE (Manchester City inspired)
        if team_name == "Manchester Blue":
            return [
                # Goalkeepers
                {"name": "Eduardo Morales", "position": Position.GOALKEEPER, "overall_rating": 89, "pace": 87, "shooting": 15, "passing": 93, "defending": 88, "physicality": 88, "age": 31, "nationality": "Brazil", "value": 40000000, "stamina": 85, "skill_moves": 1, "weak_foot": 2},
                {"name": "Stefan Ortega", "position": Position.GOALKEEPER, "overall_rating": 76, "pace": 44, "shooting": 13, "passing": 78, "defending": 77, "physicality": 74, "age": 31, "nationality": "Germany", "value": 8000000, "stamina": 82, "skill_moves": 1, "weak_foot": 2},
                {"name": "Scott Carson", "position": Position.GOALKEEPER, "overall_rating": 70, "pace": 38, "shooting": 12, "passing": 65, "defending": 71, "physicality": 72, "age": 39, "nationality": "England", "value": 500000, "stamina": 78, "skill_moves": 1, "weak_foot": 2},
                
                # Defenders
                {"name": "Jose Guardado", "position": Position.DEFENDER, "overall_rating": 84, "pace": 82, "shooting": 61, "passing": 84, "defending": 85, "physicality": 82, "age": 22, "nationality": "Croatia", "value": 80000000, "stamina": 85, "skill_moves": 3, "weak_foot": 4},
                {"name": "Manuel Akoma", "position": Position.DEFENDER, "overall_rating": 83, "pace": 76, "shooting": 39, "passing": 80, "defending": 85, "physicality": 83, "age": 29, "nationality": "Switzerland", "value": 35000000, "stamina": 86, "skill_moves": 2, "weak_foot": 3},
                {"name": "Roberto Diaz", "position": Position.DEFENDER, "overall_rating": 88, "pace": 68, "shooting": 54, "passing": 85, "defending": 91, "physicality": 88, "age": 27, "nationality": "Portugal", "value": 80000000, "stamina": 84, "skill_moves": 2, "weak_foot": 3},
                {"name": "Nathan Ake", "position": Position.DEFENDER, "overall_rating": 82, "pace": 78, "shooting": 43, "passing": 82, "defending": 84, "physicality": 80, "age": 29, "nationality": "Netherlands", "value": 40000000, "stamina": 85, "skill_moves": 3, "weak_foot": 4},
                {"name": "John Stones", "position": Position.DEFENDER, "overall_rating": 85, "pace": 73, "shooting": 55, "passing": 89, "defending": 86, "physicality": 80, "age": 30, "nationality": "England", "value": 45000000, "stamina": 83, "skill_moves": 3, "weak_foot": 3},
                {"name": "Kyle Walker", "position": Position.DEFENDER, "overall_rating": 84, "pace": 90, "shooting": 58, "passing": 75, "defending": 82, "physicality": 78, "age": 34, "nationality": "England", "value": 25000000, "stamina": 87, "skill_moves": 2, "weak_foot": 3},
                {"name": "Rico Lewis", "position": Position.DEFENDER, "overall_rating": 75, "pace": 78, "shooting": 64, "passing": 81, "defending": 72, "physicality": 65, "age": 20, "nationality": "England", "value": 20000000, "stamina": 85, "skill_moves": 3, "weak_foot": 3},
                {"name": "Sergio Gomez", "position": Position.DEFENDER, "overall_rating": 76, "pace": 84, "shooting": 69, "passing": 82, "defending": 71, "physicality": 62, "age": 24, "nationality": "Spain", "value": 18000000, "stamina": 87, "skill_moves": 4, "weak_foot": 3},
                
                # Midfielders
                {"name": "Rodrigo Hernandez", "position": Position.MIDFIELDER, "overall_rating": 89, "pace": 59, "shooting": 74, "passing": 90, "defending": 88, "physicality": 86, "age": 28, "nationality": "Spain", "value": 120000000, "stamina": 89, "skill_moves": 3, "weak_foot": 4},
                {"name": "Bruno Silva", "position": Position.MIDFIELDER, "overall_rating": 86, "pace": 74, "shooting": 79, "passing": 89, "defending": 66, "physicality": 68, "age": 30, "nationality": "Portugal", "value": 70000000, "stamina": 93, "skill_moves": 4, "weak_foot": 4},
                {"name": "Marco Kovacic", "position": Position.MIDFIELDER, "overall_rating": 84, "pace": 74, "shooting": 71, "passing": 87, "defending": 78, "physicality": 72, "age": 30, "nationality": "Croatia", "value": 35000000, "stamina": 89, "skill_moves": 4, "weak_foot": 3},
                {"name": "Mateus Nunes", "position": Position.MIDFIELDER, "overall_rating": 79, "pace": 83, "shooting": 74, "passing": 80, "defending": 69, "physicality": 76, "age": 26, "nationality": "Portugal", "value": 40000000, "stamina": 87, "skill_moves": 4, "weak_foot": 3},
                {"name": "Karl De Berg", "position": Position.MIDFIELDER, "overall_rating": 91, "pace": 76, "shooting": 88, "passing": 96, "defending": 64, "physicality": 78, "age": 33, "nationality": "Belgium", "value": 85000000, "stamina": 84, "skill_moves": 4, "weak_foot": 5},
                {"name": "Ivan Gundogan", "position": Position.MIDFIELDER, "overall_rating": 85, "pace": 63, "shooting": 81, "passing": 91, "defending": 73, "physicality": 69, "age": 34, "nationality": "Germany", "value": 25000000, "stamina": 85, "skill_moves": 4, "weak_foot": 4},
                {"name": "James McAtee", "position": Position.MIDFIELDER, "overall_rating": 74, "pace": 75, "shooting": 76, "passing": 79, "defending": 55, "physicality": 61, "age": 22, "nationality": "England", "value": 20000000, "stamina": 84, "skill_moves": 4, "weak_foot": 3},
                {"name": "Kalvin Phillips", "position": Position.MIDFIELDER, "overall_rating": 80, "pace": 63, "shooting": 66, "passing": 84, "defending": 81, "physicality": 82, "age": 28, "nationality": "England", "value": 25000000, "stamina": 87, "skill_moves": 2, "weak_foot": 3},
                
                # Forwards
                {"name": "Erik Halberg", "position": Position.FORWARD, "overall_rating": 91, "pace": 89, "shooting": 94, "passing": 65, "defending": 45, "physicality": 88, "age": 24, "nationality": "Norway", "value": 180000000, "stamina": 88, "skill_moves": 3, "weak_foot": 3},
                {"name": "Jack Grealish", "position": Position.FORWARD, "overall_rating": 84, "pace": 80, "shooting": 74, "passing": 85, "defending": 43, "physicality": 65, "age": 29, "nationality": "England", "value": 65000000, "stamina": 85, "skill_moves": 4, "weak_foot": 3},
                {"name": "Ricardo Mahrez", "position": Position.FORWARD, "overall_rating": 86, "pace": 80, "shooting": 84, "passing": 82, "defending": 38, "physicality": 61, "age": 33, "nationality": "Algeria", "value": 35000000, "stamina": 82, "skill_moves": 5, "weak_foot": 4},
                {"name": "Julio Alvarez", "position": Position.FORWARD, "overall_rating": 82, "pace": 85, "shooting": 83, "passing": 78, "defending": 58, "physicality": 70, "age": 24, "nationality": "Argentina", "value": 70000000, "stamina": 91, "skill_moves": 4, "weak_foot": 4},
                {"name": "Jeremy Doku", "position": Position.FORWARD, "overall_rating": 78, "pace": 93, "shooting": 68, "passing": 74, "defending": 32, "physicality": 65, "age": 22, "nationality": "Belgium", "value": 35000000, "stamina": 85, "skill_moves": 4, "weak_foot": 3},
                {"name": "Oscar Bobb", "position": Position.FORWARD, "overall_rating": 72, "pace": 82, "shooting": 71, "passing": 76, "defending": 28, "physicality": 58, "age": 21, "nationality": "Norway", "value": 12000000, "stamina": 84, "skill_moves": 3, "weak_foot": 3}
            ]
        
        # MADRID WHITE (Real Madrid inspired)
        elif team_name == "Madrid White":
            return [
                # Goalkeepers
                {"name": "Tiago Courtois", "position": Position.GOALKEEPER, "overall_rating": 90, "pace": 41, "shooting": 17, "passing": 75, "defending": 90, "physicality": 89, "age": 32, "nationality": "Belgium", "value": 60000000, "stamina": 85, "skill_moves": 1, "weak_foot": 2},
                {"name": "Andre Lunin", "position": Position.GOALKEEPER, "overall_rating": 77, "pace": 48, "shooting": 14, "passing": 72, "defending": 78, "physicality": 76, "age": 25, "nationality": "Ukraine", "value": 15000000, "stamina": 82, "skill_moves": 1, "weak_foot": 2},
                {"name": "Kike Arrizabalaga", "position": Position.GOALKEEPER, "overall_rating": 79, "pace": 52, "shooting": 19, "passing": 78, "defending": 79, "physicality": 71, "age": 30, "nationality": "Spain", "value": 20000000, "stamina": 84, "skill_moves": 1, "weak_foot": 3},
                
                # Defenders
                {"name": "Daniel Carvajal", "position": Position.DEFENDER, "overall_rating": 84, "pace": 78, "shooting": 68, "passing": 82, "defending": 85, "physicality": 79, "age": 32, "nationality": "Spain", "value": 25000000, "stamina": 88, "skill_moves": 3, "weak_foot": 3},
                {"name": "David Alaba", "position": Position.DEFENDER, "overall_rating": 86, "pace": 71, "shooting": 74, "passing": 88, "defending": 85, "physicality": 78, "age": 32, "nationality": "Austria", "value": 40000000, "stamina": 81, "skill_moves": 4, "weak_foot": 4},
                {"name": "Antonio Rudiger", "position": Position.DEFENDER, "overall_rating": 87, "pace": 82, "shooting": 55, "passing": 73, "defending": 88, "physicality": 86, "age": 31, "nationality": "Germany", "value": 45000000, "stamina": 89, "skill_moves": 2, "weak_foot": 3},
                {"name": "Eduardo Militao", "position": Position.DEFENDER, "overall_rating": 85, "pace": 81, "shooting": 39, "passing": 71, "defending": 86, "physicality": 82, "age": 26, "nationality": "Brazil", "value": 70000000, "stamina": 86, "skill_moves": 2, "weak_foot": 3},
                {"name": "Fernando Mendy", "position": Position.DEFENDER, "overall_rating": 82, "pace": 88, "shooting": 39, "passing": 74, "defending": 82, "physicality": 82, "age": 29, "nationality": "France", "value": 35000000, "stamina": 86, "skill_moves": 3, "weak_foot": 3},
                {"name": "Lucas Vazquez", "position": Position.DEFENDER, "overall_rating": 79, "pace": 77, "shooting": 74, "passing": 79, "defending": 76, "physicality": 71, "age": 33, "nationality": "Spain", "value": 8000000, "stamina": 91, "skill_moves": 3, "weak_foot": 3},
                {"name": "Nacho Fernandez", "position": Position.DEFENDER, "overall_rating": 80, "pace": 70, "shooting": 58, "passing": 75, "defending": 82, "physicality": 78, "age": 34, "nationality": "Spain", "value": 5000000, "stamina": 85, "skill_moves": 2, "weak_foot": 3},
                {"name": "Franco Garcia", "position": Position.DEFENDER, "overall_rating": 76, "pace": 84, "shooting": 52, "passing": 79, "defending": 74, "physicality": 68, "age": 25, "nationality": "Spain", "value": 15000000, "stamina": 87, "skill_moves": 3, "weak_foot": 3},
                
                # Midfielders
                {"name": "Lucas Modric", "position": Position.MIDFIELDER, "overall_rating": 88, "pace": 68, "shooting": 76, "passing": 91, "defending": 72, "physicality": 65, "age": 39, "nationality": "Croatia", "value": 10000000, "stamina": 88, "skill_moves": 4, "weak_foot": 4},
                {"name": "Toni Kroos", "position": Position.MIDFIELDER, "overall_rating": 88, "pace": 54, "shooting": 81, "passing": 94, "defending": 71, "physicality": 70, "age": 34, "nationality": "Germany", "value": 15000000, "stamina": 86, "skill_moves": 4, "weak_foot": 4},
                {"name": "Federico Valverde", "position": Position.MIDFIELDER, "overall_rating": 87, "pace": 80, "shooting": 84, "passing": 84, "defending": 76, "physicality": 82, "age": 26, "nationality": "Uruguay", "value": 100000000, "stamina": 95, "skill_moves": 3, "weak_foot": 4},
                {"name": "Eduardo Camavinga", "position": Position.MIDFIELDER, "overall_rating": 82, "pace": 82, "shooting": 64, "passing": 81, "defending": 78, "physicality": 78, "age": 22, "nationality": "France", "value": 80000000, "stamina": 91, "skill_moves": 4, "weak_foot": 3},
                {"name": "Aurelien Tchouameni", "position": Position.MIDFIELDER, "overall_rating": 84, "pace": 68, "shooting": 72, "passing": 80, "defending": 84, "physicality": 87, "age": 24, "nationality": "France", "value": 90000000, "stamina": 89, "skill_moves": 3, "weak_foot": 3},
                {"name": "Jake Bellmont", "position": Position.MIDFIELDER, "overall_rating": 87, "pace": 75, "shooting": 83, "passing": 83, "defending": 72, "physicality": 82, "age": 21, "nationality": "England", "value": 150000000, "stamina": 92, "skill_moves": 4, "weak_foot": 4},
                {"name": "Daniel Ceballos", "position": Position.MIDFIELDER, "overall_rating": 78, "pace": 71, "shooting": 73, "passing": 84, "defending": 68, "physicality": 66, "age": 28, "nationality": "Spain", "value": 15000000, "stamina": 84, "skill_moves": 4, "weak_foot": 3},
                {"name": "Arda Guler", "position": Position.MIDFIELDER, "overall_rating": 74, "pace": 74, "shooting": 76, "passing": 82, "defending": 42, "physicality": 56, "age": 19, "nationality": "Turkey", "value": 25000000, "stamina": 78, "skill_moves": 4, "weak_foot": 4},
                
                # Forwards
                {"name": "Kyle Morrison", "position": Position.FORWARD, "overall_rating": 91, "pace": 97, "shooting": 89, "passing": 80, "defending": 39, "physicality": 77, "age": 26, "nationality": "France", "value": 180000000, "stamina": 92, "skill_moves": 5, "weak_foot": 4},
                {"name": "Victor Santos", "position": Position.FORWARD, "overall_rating": 89, "pace": 95, "shooting": 83, "passing": 75, "defending": 29, "physicality": 68, "age": 24, "nationality": "Brazil", "value": 150000000, "stamina": 87, "skill_moves": 5, "weak_foot": 3},
                {"name": "Rodrigo Goes", "position": Position.FORWARD, "overall_rating": 85, "pace": 91, "shooting": 82, "passing": 78, "defending": 43, "physicality": 65, "age": 24, "nationality": "Brazil", "value": 80000000, "stamina": 85, "skill_moves": 4, "weak_foot": 4},
                {"name": "Endrick Felipe", "position": Position.FORWARD, "overall_rating": 77, "pace": 84, "shooting": 79, "passing": 68, "defending": 25, "physicality": 73, "age": 18, "nationality": "Brazil", "value": 40000000, "stamina": 82, "skill_moves": 4, "weak_foot": 3},
                {"name": "Brahim Diaz", "position": Position.FORWARD, "overall_rating": 80, "pace": 83, "shooting": 78, "passing": 81, "defending": 35, "physicality": 58, "age": 25, "nationality": "Morocco", "value": 30000000, "stamina": 83, "skill_moves": 4, "weak_foot": 3},
                {"name": "Jose Mato", "position": Position.FORWARD, "overall_rating": 78, "pace": 65, "shooting": 82, "passing": 72, "defending": 42, "physicality": 84, "age": 34, "nationality": "Spain", "value": 8000000, "stamina": 79, "skill_moves": 2, "weak_foot": 3}
            ]
        
        # BARCELONA FC (enhanced with star players)
        elif team_name == "Barcelona FC":
            return [
                # Goalkeepers
                {"name": "Marcus ter Stefan", "position": Position.GOALKEEPER, "overall_rating": 89, "pace": 43, "shooting": 16, "passing": 84, "defending": 90, "physicality": 82, "age": 32, "nationality": "Germany", "value": 50000000, "stamina": 85, "skill_moves": 1, "weak_foot": 2, "special_abilities": ["goalkeeper_reflexes", "distribution_master"]},
                {"name": "Inaki Pena", "position": Position.GOALKEEPER, "overall_rating": 74, "pace": 48, "shooting": 13, "passing": 76, "defending": 75, "physicality": 73, "age": 25, "nationality": "Spain", "value": 8000000, "stamina": 82, "skill_moves": 1, "weak_foot": 2, "special_abilities": []},
                {"name": "Andre Astralaga", "position": Position.GOALKEEPER, "overall_rating": 67, "pace": 45, "shooting": 12, "passing": 71, "defending": 68, "physicality": 69, "age": 20, "nationality": "Spain", "value": 2000000, "stamina": 80, "skill_moves": 1, "weak_foot": 2, "special_abilities": []},
                
                # Defenders
                {"name": "Ronald Araujo", "position": Position.DEFENDER, "overall_rating": 85, "pace": 82, "shooting": 48, "passing": 71, "defending": 87, "physicality": 89, "age": 25, "nationality": "Uruguay", "value": 70000000, "stamina": 87, "skill_moves": 2, "weak_foot": 3, "special_abilities": ["aerial_dominance", "last_man_defending"]},
                {"name": "Jules Kounde", "position": Position.DEFENDER, "overall_rating": 84, "pace": 83, "shooting": 43, "passing": 78, "defending": 85, "physicality": 78, "age": 26, "nationality": "France", "value": 65000000, "stamina": 86, "skill_moves": 3, "weak_foot": 3, "special_abilities": ["pace_defending", "ball_playing_defender"]},
                {"name": "Andreas Christensen", "position": Position.DEFENDER, "overall_rating": 81, "pace": 74, "shooting": 41, "passing": 82, "defending": 83, "physicality": 77, "age": 28, "nationality": "Denmark", "value": 35000000, "stamina": 84, "skill_moves": 2, "weak_foot": 4, "special_abilities": ["ball_playing_defender"]},
                {"name": "Alex Balde", "position": Position.DEFENDER, "overall_rating": 78, "pace": 87, "shooting": 52, "passing": 76, "defending": 76, "physicality": 71, "age": 21, "nationality": "Spain", "value": 35000000, "stamina": 89, "skill_moves": 3, "weak_foot": 3, "special_abilities": ["lightning_pace", "attacking_fullback"]},
                {"name": "Joao Cancelo", "position": Position.DEFENDER, "overall_rating": 86, "pace": 84, "shooting": 74, "passing": 87, "defending": 77, "physicality": 73, "age": 30, "nationality": "Portugal", "value": 45000000, "stamina": 85, "skill_moves": 4, "weak_foot": 4, "special_abilities": ["attacking_fullback", "versatility_master"]},
                {"name": "Marco Alonso", "position": Position.DEFENDER, "overall_rating": 77, "pace": 68, "shooting": 76, "passing": 79, "defending": 78, "physicality": 82, "age": 33, "nationality": "Spain", "value": 12000000, "stamina": 81, "skill_moves": 3, "weak_foot": 4, "special_abilities": ["free_kick_specialist"]},
                {"name": "Sergio Roberto", "position": Position.DEFENDER, "overall_rating": 76, "pace": 73, "shooting": 68, "passing": 83, "defending": 74, "physicality": 71, "age": 32, "nationality": "Spain", "value": 8000000, "stamina": 88, "skill_moves": 3, "weak_foot": 3, "special_abilities": ["versatility_master"]},
                {"name": "Inigo Martinez", "position": Position.DEFENDER, "overall_rating": 82, "pace": 66, "shooting": 45, "passing": 78, "defending": 86, "physicality": 84, "age": 33, "nationality": "Spain", "value": 15000000, "stamina": 83, "skill_moves": 2, "weak_foot": 4, "special_abilities": ["ball_playing_defender", "leadership"]},
                
                # Midfielders
                {"name": "Pablo Gonzales", "position": Position.MIDFIELDER, "overall_rating": 87, "pace": 74, "shooting": 76, "passing": 90, "defending": 59, "physicality": 66, "age": 22, "nationality": "Spain", "value": 100000000, "stamina": 88, "skill_moves": 4, "weak_foot": 4, "special_abilities": ["vision_master", "press_resistant", "la_masia_magic"]},
                {"name": "Franco de Jong", "position": Position.MIDFIELDER, "overall_rating": 87, "pace": 78, "shooting": 72, "passing": 89, "defending": 78, "physicality": 79, "age": 27, "nationality": "Netherlands", "value": 80000000, "stamina": 92, "skill_moves": 4, "weak_foot": 4, "special_abilities": ["press_resistant", "box_to_box_master", "dribbling_midfielder"]},
                {"name": "Gavi", "position": Position.MIDFIELDER, "overall_rating": 84, "pace": 79, "shooting": 68, "passing": 86, "defending": 71, "physicality": 68, "age": 20, "nationality": "Spain", "value": 90000000, "stamina": 94, "skill_moves": 4, "weak_foot": 3, "special_abilities": ["press_resistant", "young_phenomenon", "la_masia_magic"]},
                {"name": "Ivan Gundogan", "position": Position.MIDFIELDER, "overall_rating": 85, "pace": 63, "shooting": 81, "passing": 91, "defending": 73, "physicality": 69, "age": 34, "nationality": "Germany", "value": 25000000, "stamina": 85, "skill_moves": 4, "weak_foot": 4, "special_abilities": ["vision_master", "free_kick_specialist", "leadership"]},
                {"name": "Oriol Romeu", "position": Position.MIDFIELDER, "overall_rating": 78, "pace": 62, "shooting": 58, "passing": 81, "defending": 83, "physicality": 81, "age": 32, "nationality": "Spain", "value": 12000000, "stamina": 87, "skill_moves": 2, "weak_foot": 3, "special_abilities": ["defensive_midfielder"]},
                {"name": "Fermin Lopez", "position": Position.MIDFIELDER, "overall_rating": 76, "pace": 77, "shooting": 73, "passing": 82, "defending": 65, "physicality": 70, "age": 21, "nationality": "Spain", "value": 20000000, "stamina": 86, "skill_moves": 3, "weak_foot": 3, "special_abilities": ["young_phenomenon", "la_masia_magic"]},
                {"name": "Pablo Torre", "position": Position.MIDFIELDER, "overall_rating": 74, "pace": 71, "shooting": 75, "passing": 84, "defending": 52, "physicality": 61, "age": 21, "nationality": "Spain", "value": 15000000, "stamina": 82, "skill_moves": 4, "weak_foot": 3, "special_abilities": ["young_phenomenon", "creative_playmaker"]},
                {"name": "Marc Casado", "position": Position.MIDFIELDER, "overall_rating": 72, "pace": 69, "shooting": 64, "passing": 79, "defending": 74, "physicality": 68, "age": 21, "nationality": "Spain", "value": 8000000, "stamina": 84, "skill_moves": 3, "weak_foot": 3, "special_abilities": ["young_phenomenon", "la_masia_magic"]},
                
                # Forwards
                {"name": "Robert Lewanski", "position": Position.FORWARD, "overall_rating": 90, "pace": 77, "shooting": 94, "passing": 79, "defending": 43, "physicality": 84, "age": 36, "nationality": "Poland", "value": 35000000, "stamina": 82, "skill_moves": 4, "weak_foot": 4, "special_abilities": ["clinical_finisher", "penalty_master", "target_man", "veteran_experience"]},
                {"name": "Raphinha", "position": Position.FORWARD, "overall_rating": 84, "pace": 85, "shooting": 82, "passing": 78, "defending": 37, "physicality": 67, "age": 28, "nationality": "Brazil", "value": 60000000, "stamina": 87, "skill_moves": 4, "weak_foot": 4, "special_abilities": ["brazilian_flair", "wing_wizard", "pace_merchant"]},
                {"name": "Joao Felix", "position": Position.FORWARD, "overall_rating": 83, "pace": 83, "shooting": 81, "passing": 82, "defending": 31, "physicality": 65, "age": 25, "nationality": "Portugal", "value": 70000000, "stamina": 84, "skill_moves": 5, "weak_foot": 4, "special_abilities": ["technical_dribbler", "creative_forward", "luxury_player"]},
                {"name": "Fernando Torres", "position": Position.FORWARD, "overall_rating": 82, "pace": 86, "shooting": 81, "passing": 76, "defending": 42, "physicality": 69, "age": 24, "nationality": "Spain", "value": 50000000, "stamina": 88, "skill_moves": 3, "weak_foot": 3, "special_abilities": ["pace_merchant", "versatile_forward"]},
                {"name": "Antonio Fati", "position": Position.FORWARD, "overall_rating": 80, "pace": 84, "shooting": 79, "passing": 74, "defending": 26, "physicality": 62, "age": 22, "nationality": "Spain", "value": 40000000, "stamina": 81, "skill_moves": 4, "weak_foot": 3, "special_abilities": ["young_phenomenon", "la_masia_magic", "injury_prone"]},
                {"name": "Vitor Roque", "position": Position.FORWARD, "overall_rating": 75, "pace": 81, "shooting": 76, "passing": 68, "defending": 22, "physicality": 72, "age": 19, "nationality": "Brazil", "value": 25000000, "stamina": 83, "skill_moves": 3, "weak_foot": 3, "special_abilities": ["young_phenomenon", "brazilian_prospect"]}
            ]
        
        # LIVERPOOL RED (enhanced with star players)  
        elif team_name == "Liverpool Red":
            return [
                # Goalkeepers
                {"name": "Alex Becker", "position": Position.GOALKEEPER, "overall_rating": 90, "pace": 51, "shooting": 18, "passing": 80, "defending": 91, "physicality": 86, "age": 31, "nationality": "Brazil", "value": 55000000, "stamina": 86, "skill_moves": 1, "weak_foot": 3, "special_abilities": ["goalkeeper_reflexes", "distribution_master", "sweeper_keeper"]},
                {"name": "Caoimhin Kelleher", "position": Position.GOALKEEPER, "overall_rating": 79, "pace": 49, "shooting": 15, "passing": 75, "defending": 80, "physicality": 78, "age": 26, "nationality": "Ireland", "value": 18000000, "stamina": 83, "skill_moves": 1, "weak_foot": 2, "special_abilities": ["young_keeper"]},
                {"name": "Adrian San Miguel", "position": Position.GOALKEEPER, "overall_rating": 71, "pace": 42, "shooting": 14, "passing": 68, "defending": 72, "physicality": 74, "age": 37, "nationality": "Spain", "value": 2000000, "stamina": 79, "skill_moves": 1, "weak_foot": 2, "special_abilities": ["veteran_experience"]},
                
                # Defenders
                {"name": "Vincent van Berg", "position": Position.DEFENDER, "overall_rating": 89, "pace": 76, "shooting": 62, "passing": 91, "defending": 92, "physicality": 86, "age": 33, "nationality": "Netherlands", "value": 55000000, "stamina": 84, "skill_moves": 2, "weak_foot": 3, "special_abilities": ["aerial_dominance", "captain_influence", "ball_playing_defender", "leadership"]},
                {"name": "Mohamed Saladin", "position": Position.DEFENDER, "overall_rating": 84, "pace": 82, "shooting": 55, "passing": 85, "defending": 86, "physicality": 83, "age": 26, "nationality": "Egypt", "value": 60000000, "stamina": 87, "skill_moves": 2, "weak_foot": 3, "special_abilities": ["pace_defending", "ball_playing_defender"]},
                {"name": "Andrew Robertson", "position": Position.DEFENDER, "overall_rating": 86, "pace": 86, "shooting": 59, "passing": 84, "defending": 85, "physicality": 78, "age": 30, "nationality": "Scotland", "value": 40000000, "stamina": 92, "skill_moves": 3, "weak_foot": 4, "special_abilities": ["attacking_fullback", "crossing_specialist", "stamina_monster"]},
                {"name": "Trent Alexander-Arnold", "position": Position.DEFENDER, "overall_rating": 87, "pace": 76, "shooting": 75, "passing": 93, "defending": 78, "physicality": 71, "age": 26, "nationality": "England", "value": 80000000, "stamina": 89, "skill_moves": 4, "weak_foot": 4, "special_abilities": ["crossing_specialist", "free_kick_specialist", "vision_master", "attacking_fullback"]},
                {"name": "Joel Matip", "position": Position.DEFENDER, "overall_rating": 81, "pace": 68, "shooting": 48, "passing": 79, "defending": 84, "physicality": 87, "age": 33, "nationality": "Cameroon", "value": 15000000, "stamina": 82, "skill_moves": 2, "weak_foot": 3, "special_abilities": ["aerial_dominance", "ball_playing_defender"]},
                {"name": "Kostas Tsimikas", "position": Position.DEFENDER, "overall_rating": 77, "pace": 82, "shooting": 56, "passing": 78, "defending": 76, "physicality": 73, "age": 28, "nationality": "Greece", "value": 20000000, "stamina": 88, "skill_moves": 3, "weak_foot": 3, "special_abilities": ["attacking_fullback"]},
                {"name": "Conor Bradley", "position": Position.DEFENDER, "overall_rating": 73, "pace": 84, "shooting": 62, "passing": 75, "defending": 71, "physicality": 68, "age": 21, "nationality": "Northern Ireland", "value": 12000000, "stamina": 87, "skill_moves": 3, "weak_foot": 3, "special_abilities": ["young_phenomenon", "attacking_fullback"]},
                {"name": "Joe Gomez", "position": Position.DEFENDER, "overall_rating": 79, "pace": 78, "shooting": 44, "passing": 74, "defending": 81, "physicality": 82, "age": 27, "nationality": "England", "value": 25000000, "stamina": 85, "skill_moves": 2, "weak_foot": 3, "special_abilities": ["versatility_master", "pace_defending"]},
                
                # Midfielders
                {"name": "Fabricio Tavares", "position": Position.MIDFIELDER, "overall_rating": 85, "pace": 68, "shooting": 73, "passing": 86, "defending": 88, "physicality": 83, "age": 31, "nationality": "Brazil", "value": 35000000, "stamina": 89, "skill_moves": 3, "weak_foot": 4, "special_abilities": ["defensive_midfielder", "brazilian_flair", "leadership"]},
                {"name": "Jordan Henderson", "position": Position.MIDFIELDER, "overall_rating": 81, "pace": 67, "shooting": 68, "passing": 86, "defending": 79, "physicality": 76, "age": 34, "nationality": "England", "value": 12000000, "stamina": 91, "skill_moves": 3, "weak_foot": 3, "special_abilities": ["captain_influence", "leadership", "veteran_experience", "stamina_monster"]},
                {"name": "Thiago Alcantara", "position": Position.MIDFIELDER, "overall_rating": 86, "pace": 68, "shooting": 76, "passing": 92, "defending": 75, "physicality": 70, "age": 33, "nationality": "Spain", "value": 20000000, "stamina": 81, "skill_moves": 5, "weak_foot": 4, "special_abilities": ["vision_master", "press_resistant", "technical_master", "injury_prone"]},
                {"name": "Curtis Jones", "position": Position.MIDFIELDER, "overall_rating": 78, "pace": 75, "shooting": 72, "passing": 81, "defending": 69, "physicality": 71, "age": 23, "nationality": "England", "value": 25000000, "stamina": 86, "skill_moves": 4, "weak_foot": 3, "special_abilities": ["young_phenomenon", "academy_graduate"]},
                {"name": "Harvey Elliott", "position": Position.MIDFIELDER, "overall_rating": 76, "pace": 77, "shooting": 74, "passing": 82, "defending": 58, "physicality": 62, "age": 21, "nationality": "England", "value": 30000000, "stamina": 84, "skill_moves": 4, "weak_foot": 4, "special_abilities": ["young_phenomenon", "creative_playmaker", "academy_graduate"]},
                {"name": "Stefan Bajcetic", "position": Position.MIDFIELDER, "overall_rating": 72, "pace": 71, "shooting": 65, "passing": 76, "defending": 74, "physicality": 68, "age": 20, "nationality": "Spain", "value": 15000000, "stamina": 83, "skill_moves": 3, "weak_foot": 3, "special_abilities": ["young_phenomenon", "academy_graduate"]},
                {"name": "Alex Mac Allister", "position": Position.MIDFIELDER, "overall_rating": 83, "pace": 73, "shooting": 78, "passing": 87, "defending": 75, "physicality": 74, "age": 25, "nationality": "Argentina", "value": 60000000, "stamina": 88, "skill_moves": 4, "weak_foot": 4, "special_abilities": ["box_to_box_master", "world_cup_winner"]},
                {"name": "Dominik Szoboszlai", "position": Position.MIDFIELDER, "overall_rating": 81, "pace": 78, "shooting": 82, "passing": 84, "defending": 68, "physicality": 75, "age": 24, "nationality": "Hungary", "value": 55000000, "stamina": 87, "skill_moves": 4, "weak_foot": 4, "special_abilities": ["free_kick_specialist", "creative_playmaker", "hungarian_talent"]},
                
                # Forwards
                {"name": "Mohamed Saladin", "position": Position.FORWARD, "overall_rating": 90, "pace": 90, "shooting": 89, "passing": 81, "defending": 45, "physicality": 75, "age": 32, "nationality": "Egypt", "value": 65000000, "stamina": 88, "skill_moves": 4, "weak_foot": 4, "special_abilities": ["pace_merchant", "clinical_finisher", "penalty_master", "left_foot_wizard", "pharaoh_magic"]},
                {"name": "Sadio Mane", "position": Position.FORWARD, "overall_rating": 88, "pace": 91, "shooting": 85, "passing": 76, "defending": 44, "physicality": 77, "age": 32, "nationality": "Senegal", "value": 50000000, "stamina": 92, "skill_moves": 4, "weak_foot": 3, "special_abilities": ["lightning_pace", "african_warrior", "big_game_player", "stamina_monster"]},
                {"name": "Darwin Nunez", "position": Position.FORWARD, "overall_rating": 82, "pace": 89, "shooting": 83, "passing": 68, "defending": 38, "physicality": 82, "age": 25, "nationality": "Uruguay", "value": 75000000, "stamina": 89, "skill_moves": 3, "weak_foot": 3, "special_abilities": ["pace_merchant", "aerial_dominance", "south_american_flair"]},
                {"name": "Diego Jota", "position": Position.FORWARD, "overall_rating": 84, "pace": 84, "shooting": 85, "passing": 74, "defending": 43, "physicality": 71, "age": 28, "nationality": "Portugal", "value": 55000000, "stamina": 86, "skill_moves": 4, "weak_foot": 4, "special_abilities": ["versatile_forward", "clinical_finisher", "big_game_player"]},
                {"name": "Luis Diaz", "position": Position.FORWARD, "overall_rating": 84, "pace": 88, "shooting": 78, "passing": 76, "defending": 39, "physicality": 70, "age": 27, "nationality": "Colombia", "value": 65000000, "stamina": 87, "skill_moves": 4, "weak_foot": 3, "special_abilities": ["pace_merchant", "south_american_flair", "wing_wizard"]},
                {"name": "Cody Gakpo", "position": Position.FORWARD, "overall_rating": 81, "pace": 82, "shooting": 80, "passing": 78, "defending": 35, "physicality": 73, "age": 25, "nationality": "Netherlands", "value": 50000000, "stamina": 85, "skill_moves": 4, "weak_foot": 4, "special_abilities": ["versatile_forward", "dutch_technique", "aerial_threat"]}
            ]
        
        # Default template for other teams (will generate based on team rating)
        else:
            return self.generate_default_players(team_name, team_rating)
    
    def generate_default_players(self, team_name: str, team_rating: int) -> list:
        """Generate default players when specific templates aren't available"""
        import random
        
        # Generic player name pools (copyright-safe)
        first_names = [
            "Alex", "Marco", "David", "Carlos", "João", "Miguel", "Antonio", "Luis", "Fernando", "Diego",
            "André", "Pedro", "Rafael", "Gabriel", "Daniel", "Ricardo", "Paulo", "Bruno", "Sergio", "Manuel",
            "José", "Francisco", "Roberto", "Eduardo", "Adrián", "Alejandro", "Gonzalo", "Martín", "Nicolás", "Sebastián"
        ]
        
        last_names = [
            "Silva", "Santos", "Oliveira", "Pereira", "Costa", "Rodrigues", "Martins", "Jesus", "Sousa", "Fernandes",
            "Gonçalves", "Gomes", "Lopes", "Marques", "Alves", "Almeida", "Ribeiro", "Pinto", "Carvalho", "Teixeira",
            "Moreira", "Ferreira", "Dias", "Mendes", "Nunes", "Correia", "Reis", "Antunes", "Fonseca", "Pires"
        ]
        
        countries = [
            "Portugal", "Spain", "Brazil", "Argentina", "France", "Italy", "Germany", "England", 
            "Netherlands", "Belgium", "Croatia", "Colombia", "Mexico", "Chile", "Uruguay", "Morocco"
        ]
        
        players = []
        positions_count = [
            (Position.GOALKEEPER, 3),
            (Position.DEFENDER, 8), 
            (Position.MIDFIELDER, 8),
            (Position.FORWARD, 6)
        ]
        
        for position, count in positions_count:
            for i in range(count):
                # Determine if this is a star player (first 1-2 in each position)
                is_star = i < 2
                
                base_rating = team_rating
                if is_star:
                    base_rating = team_rating + random.randint(-2, 3)
                else:
                    base_rating = team_rating - random.randint(3, 12)
                
                base_rating = max(50, min(99, base_rating))
                
                # Generate position-specific stats
                stats = self.generate_position_stats(position, base_rating, is_star)
                
                player = {
                    "name": f"{random.choice(first_names)} {random.choice(last_names)}",
                    "position": position,
                    "overall_rating": base_rating,
                    "pace": stats["pace"],
                    "shooting": stats["shooting"],
                    "passing": stats["passing"],
                    "defending": stats["defending"],
                    "physicality": stats["physicality"],
                    "age": random.randint(18, 35),
                    "nationality": random.choice(countries),
                    "value": self.calculate_player_value(base_rating, position, random.randint(18, 35)),
                    "stamina": random.randint(75, 95),
                    "skill_moves": 5 if is_star and position == Position.FORWARD else random.randint(2, 4),
                    "weak_foot": random.randint(2, 4)
                }
                players.append(player)
        
        return players
    
    def generate_position_stats(self, position: Position, base_rating: int, is_star: bool = False) -> dict:
        """Generate realistic stats based on position"""
        import random
        
        def safe_randint(min_val, max_val):
            """Safely generate random int ensuring min <= max"""
            min_val = max(1, min_val)
            max_val = min(99, max_val)
            if min_val >= max_val:
                return min_val
            return random.randint(min_val, max_val)
        
        if position == Position.GOALKEEPER:
            return {
                "pace": safe_randint(35, 55),
                "shooting": safe_randint(15, 35),
                "passing": safe_randint(max(50, base_rating - 15), min(85, base_rating + 5)),
                "defending": safe_randint(max(75, base_rating - 5), min(95, base_rating + 5)),
                "physicality": safe_randint(max(70, base_rating - 10), min(90, base_rating + 5))
            }
        elif position == Position.DEFENDER:
            return {
                "pace": safe_randint(max(45, base_rating - 20), min(85, base_rating)),
                "shooting": safe_randint(25, 65),
                "passing": safe_randint(max(60, base_rating - 15), min(90, base_rating + 5)),
                "defending": safe_randint(max(70, base_rating - 5), min(95, base_rating + 5)),
                "physicality": safe_randint(max(70, base_rating - 10), min(95, base_rating + 5))
            }
        elif position == Position.MIDFIELDER:
            return {
                "pace": safe_randint(max(50, base_rating - 20), min(85, base_rating + 5)),
                "shooting": safe_randint(max(45, base_rating - 25), min(85, base_rating)),
                "passing": safe_randint(max(70, base_rating - 5), min(95, base_rating + 5)),
                "defending": safe_randint(max(40, base_rating - 25), min(85, base_rating)),
                "physicality": safe_randint(max(55, base_rating - 20), min(85, base_rating))
            }
        else:  # Forward
            return {
                "pace": safe_randint(max(60, base_rating - 15), min(95, base_rating + 5)),
                "shooting": safe_randint(max(70, base_rating - 5), min(95, base_rating + 5)),
                "passing": safe_randint(max(50, base_rating - 20), min(85, base_rating)),
                "defending": safe_randint(20, 50),
                "physicality": safe_randint(max(60, base_rating - 20), min(90, base_rating))
            }
    
    def calculate_player_value(self, rating: int, position: Position, age: int) -> int:
        """Calculate player market value based on rating, position, and age"""
        base_value = 0
        
        # Base value by rating
        if rating >= 90:
            base_value = 100000000  # 100M
        elif rating >= 85:
            base_value = 50000000   # 50M
        elif rating >= 80:
            base_value = 25000000   # 25M
        elif rating >= 75:
            base_value = 10000000   # 10M
        else:
            base_value = 2000000    # 2M
        
        # Age multiplier
        if age <= 21:
            age_multiplier = 1.5  # Young talent premium
        elif age <= 25:
            age_multiplier = 1.2  # Prime years approaching
        elif age <= 29:
            age_multiplier = 1.0  # Peak years
        elif age <= 32:
            age_multiplier = 0.7  # Declining years
        else:
            age_multiplier = 0.4  # Veteran
        
        # Position multiplier
        position_multipliers = {
            Position.FORWARD: 1.2,
            Position.MIDFIELDER: 1.0,
            Position.DEFENDER: 0.9,
            Position.GOALKEEPER: 0.8
        }
        
        final_value = int(base_value * age_multiplier * position_multipliers.get(position, 1.0))
        return max(100000, final_value)  # Minimum 100k value
    
    # CRUD Operations for Teams
    async def create_team(self, team: Team) -> str:
        result = await self.db.teams.insert_one(team.model_dump())
        return str(result.inserted_id)
    
    async def get_team(self, team_id: str) -> Optional[Team]:
        team_data = await self.db.teams.find_one({"id": team_id})
        return Team(**team_data) if team_data else None
    
    async def get_teams(self, skip: int = 0, limit: int = 50) -> List[Team]:
        cursor = self.db.teams.find().skip(skip).limit(limit)
        teams = await cursor.to_list(length=limit)
        return [Team(**team) for team in teams]
    
    async def get_teams_by_league(self, league: str) -> List[Team]:
        cursor = self.db.teams.find({"league": league})
        teams = await cursor.to_list(length=None)
        return [Team(**team) for team in teams]
    
    async def get_teams_by_country(self, country: str) -> List[Team]:
        cursor = self.db.teams.find({"country": country})
        teams = await cursor.to_list(length=None)
        return [Team(**team) for team in teams]
    
    async def update_team(self, team_id: str, team_data: dict) -> bool:
        result = await self.db.teams.update_one(
            {"id": team_id}, 
            {"$set": team_data}
        )
        return result.modified_count > 0
    
    async def delete_team(self, team_id: str) -> bool:
        result = await self.db.teams.delete_one({"id": team_id})
        return result.deleted_count > 0
    
    # CRUD Operations for Stadiums
    async def create_stadium(self, stadium: Stadium) -> str:
        result = await self.db.stadiums.insert_one(stadium.model_dump())
        return str(result.inserted_id)
    
    async def get_stadium(self, stadium_id: str) -> Optional[Stadium]:
        stadium_data = await self.db.stadiums.find_one({"id": stadium_id})
        return Stadium(**stadium_data) if stadium_data else None
    
    async def get_stadiums(self, skip: int = 0, limit: int = 50) -> List[Stadium]:
        cursor = self.db.stadiums.find().skip(skip).limit(limit)
        stadiums = await cursor.to_list(length=limit)
        return [Stadium(**stadium) for stadium in stadiums]
    
    async def get_stadiums_by_country(self, country: str) -> List[Stadium]:
        cursor = self.db.stadiums.find({"country": country})
        stadiums = await cursor.to_list(length=None)
        return [Stadium(**stadium) for stadium in stadiums]
    
    # CRUD Operations for User Profiles
    async def create_user_profile(self, profile: UserProfile) -> str:
        result = await self.db.user_profiles.insert_one(profile.model_dump())
        return str(result.inserted_id)
    
    async def get_user_profile(self, user_id: str) -> Optional[UserProfile]:
        profile_data = await self.db.user_profiles.find_one({"id": user_id})
        return UserProfile(**profile_data) if profile_data else None
    
    async def get_user_profile_by_username(self, username: str) -> Optional[UserProfile]:
        profile_data = await self.db.user_profiles.find_one({"username": username})
        return UserProfile(**profile_data) if profile_data else None
    
    async def update_user_profile(self, user_id: str, profile_data: dict) -> bool:
        result = await self.db.user_profiles.update_one(
            {"id": user_id}, 
            {"$set": profile_data}
        )
        return result.modified_count > 0
    
    # CRUD Operations for Matches
    async def create_match(self, match: Match) -> str:
        result = await self.db.matches.insert_one(match.model_dump())
        return str(result.inserted_id)
    
    async def get_match(self, match_id: str) -> Optional[Match]:
        match_data = await self.db.matches.find_one({"id": match_id})
        return Match(**match_data) if match_data else None
    
    async def get_matches_by_team(self, team_id: str) -> List[Match]:
        cursor = self.db.matches.find({
            "$or": [
                {"home_team_id": team_id},
                {"away_team_id": team_id}
            ]
        })
        matches = await cursor.to_list(length=None)
        return [Match(**match) for match in matches]
    
    async def get_matches_by_player(self, player_id: str) -> List[Match]:
        cursor = self.db.matches.find({"player_id": player_id})
        matches = await cursor.to_list(length=None)
        return [Match(**match) for match in matches]
    
    async def update_match(self, match_id: str, match_data: dict) -> bool:
        result = await self.db.matches.update_one(
            {"id": match_id}, 
            {"$set": match_data}
        )
        return result.modified_count > 0
    
    # CRUD Operations for Tournaments
    async def create_tournament(self, tournament: Tournament) -> str:
        result = await self.db.tournaments.insert_one(tournament.model_dump())
        return str(result.inserted_id)
    
    async def get_tournament(self, tournament_id: str) -> Optional[Tournament]:
        tournament_data = await self.db.tournaments.find_one({"id": tournament_id})
        return Tournament(**tournament_data) if tournament_data else None
    
    async def get_tournaments(self, skip: int = 0, limit: int = 20) -> List[Tournament]:
        cursor = self.db.tournaments.find().skip(skip).limit(limit)
        tournaments = await cursor.to_list(length=limit)
        return [Tournament(**tournament) for tournament in tournaments]
    
    async def update_tournament(self, tournament_id: str, tournament_data: dict) -> bool:
        result = await self.db.tournaments.update_one(
            {"id": tournament_id}, 
            {"$set": tournament_data}
        )
        return result.modified_count > 0
    
    # CRUD Operations for Career Mode
    async def create_career(self, career: Career) -> str:
        result = await self.db.careers.insert_one(career.model_dump())
        return str(result.inserted_id)
    
    async def get_career(self, career_id: str) -> Optional[Career]:
        career_data = await self.db.careers.find_one({"id": career_id})
        return Career(**career_data) if career_data else None
    
    async def get_career_by_user(self, user_id: str) -> Optional[Career]:
        career_data = await self.db.careers.find_one({"user_id": user_id})
        return Career(**career_data) if career_data else None
    
    async def update_career(self, career_id: str, career_data: dict) -> bool:
        result = await self.db.careers.update_one(
            {"id": career_id}, 
            {"$set": career_data}
        )
        return result.modified_count > 0
    
    # CRUD Operations for Achievements
    async def get_achievements(self, skip: int = 0, limit: int = 50) -> List[Achievement]:
        cursor = self.db.achievements.find().skip(skip).limit(limit)
        achievements = await cursor.to_list(length=limit)
        return [Achievement(**achievement) for achievement in achievements]
    
    async def get_achievements_by_category(self, category: str) -> List[Achievement]:
        cursor = self.db.achievements.find({"category": category})
        achievements = await cursor.to_list(length=None)
        return [Achievement(**achievement) for achievement in achievements]
    
    async def get_user_achievements(self, user_id: str) -> List[str]:
        profile = await self.get_user_profile(user_id)
        return profile.achievements if profile else []
    
    async def unlock_achievement(self, user_id: str, achievement_id: str) -> bool:
        result = await self.db.user_profiles.update_one(
            {"id": user_id},
            {"$addToSet": {"achievements": achievement_id}}
        )
        return result.modified_count > 0
    
    # Statistics and Analytics
    async def get_user_stats(self, user_id: str) -> Dict[str, Any]:
        profile = await self.get_user_profile(user_id)
        if not profile:
            return {}
        
        matches = await self.get_matches_by_player(user_id)
        
        return {
            "level": profile.level,
            "experience": profile.experience,
            "total_matches": len(matches),
            "total_wins": profile.total_wins,
            "total_draws": profile.total_draws,
            "total_losses": profile.total_losses,
            "win_rate": profile.total_wins / max(1, len(matches)) * 100,
            "goals_scored": profile.total_goals_scored,
            "goals_conceded": profile.total_goals_conceded,
            "goal_difference": profile.total_goals_scored - profile.total_goals_conceded,
            "achievements_unlocked": len(profile.achievements)
        }
    
    async def get_team_stats(self, team_id: str) -> Dict[str, Any]:
        matches = await self.get_matches_by_team(team_id)
        
        wins = sum(1 for match in matches if (
            (match.home_team_id == team_id and match.home_score > match.away_score) or
            (match.away_team_id == team_id and match.away_score > match.home_score)
        ))
        
        draws = sum(1 for match in matches if match.home_score == match.away_score)
        losses = len(matches) - wins - draws
        
        goals_scored = sum(
            match.home_score if match.home_team_id == team_id else match.away_score
            for match in matches
        )
        
        goals_conceded = sum(
            match.away_score if match.home_team_id == team_id else match.home_score
            for match in matches
        )
        
        return {
            "total_matches": len(matches),
            "wins": wins,
            "draws": draws,
            "losses": losses,
            "win_rate": wins / max(1, len(matches)) * 100,
            "goals_scored": goals_scored,
            "goals_conceded": goals_conceded,
            "goal_difference": goals_scored - goals_conceded
        }
    
    async def get_league_table(self, league: str) -> List[Dict[str, Any]]:
        teams = await self.get_teams_by_league(league)
        table = []
        
        for team in teams:
            stats = await self.get_team_stats(team.id)
            points = stats["wins"] * 3 + stats["draws"]
            
            table.append({
                "team_name": team.name,
                "team_id": team.id,
                "matches_played": stats["total_matches"],
                "wins": stats["wins"],
                "draws": stats["draws"],
                "losses": stats["losses"],
                "goals_for": stats["goals_scored"],
                "goals_against": stats["goals_conceded"],
                "goal_difference": stats["goal_difference"],
                "points": points
            })
        
        # Sort by points (descending), then by goal difference (descending)
        table.sort(key=lambda x: (x["points"], x["goal_difference"]), reverse=True)
        
        # Add position
        for i, team in enumerate(table):
            team["position"] = i + 1
        
        return table
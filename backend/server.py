from fastapi import FastAPI, APIRouter, HTTPException, Depends, Query, Path as FastAPIPath
from fastapi.middleware.cors import CORSMiddleware
from dotenv import load_dotenv
from motor.motor_asyncio import AsyncIOMotorClient
import os
import logging
from pathlib import Path
from typing import List, Optional, Dict, Any
from datetime import datetime, timedelta
import asyncio

# Import our models and database
from models import *
from database import DatabaseManager

ROOT_DIR = Path(__file__).parent
load_dotenv(ROOT_DIR / '.env')

# MongoDB connection
mongo_url = os.environ['MONGO_URL']
client = AsyncIOMotorClient(mongo_url)
db = client[os.environ['DB_NAME']]

# Initialize database manager
db_manager = DatabaseManager()

# Create the main app
app = FastAPI(title="Football Master API", version="1.0.0")

# Create API router
api_router = APIRouter(prefix="/api")

# Configure CORS
app.add_middleware(
    CORSMiddleware,
    allow_credentials=True,
    allow_origins=["*"],
    allow_methods=["*"],
    allow_headers=["*"],
)

# Configure logging
logging.basicConfig(
    level=logging.INFO,
    format='%(asctime)s - %(name)s - %(levelname)s - %(message)s'
)
logger = logging.getLogger(__name__)

# Startup event
@app.on_event("startup")
async def startup_event():
    logger.info("Initializing Football Master database...")
    await db_manager.initialize_data()
    logger.info("Database initialized successfully!")

# Root endpoint
@api_router.get("/")
async def root():
    return {"message": "Football Master API is running!", "version": "1.0.0"}

# Health check
@api_router.get("/health")
async def health_check():
    return {"status": "healthy", "timestamp": datetime.utcnow()}

# ============ USER PROFILE ENDPOINTS ============

@api_router.post("/users", response_model=dict)
async def create_user_profile(profile: UserProfile):
    """Create a new user profile"""
    try:
        user_id = await db_manager.create_user_profile(profile.dict())
        return {"user_id": user_id, "message": "User created successfully"}
    except Exception as e:
        raise HTTPException(status_code=400, detail=str(e))

@api_router.get("/users/{user_id}", response_model=UserProfile)
async def get_user_profile(user_id: str = FastAPIPath(..., description="User ID")):
    """Get user profile by ID"""
    profile = await db_manager.get_user_profile(user_id)
    if not profile:
        raise HTTPException(status_code=404, detail="User not found")
    return profile

@api_router.put("/users/{user_id}/settings")
async def update_user_settings(
    user_id: str,
    settings: Dict[str, Any]
):
    """Update user control settings"""
    try:
        await db.user_profiles.update_one(
            {"id": user_id},
            {"$set": {"control_settings": settings}}
        )
        return {"message": "Settings updated successfully"}
    except Exception as e:
        raise HTTPException(status_code=400, detail=str(e))

# ============ TEAM ENDPOINTS ============

@api_router.get("/teams", response_model=List[Team])
async def get_all_teams(
    league: Optional[str] = Query(None, description="Filter by league"),
    country: Optional[str] = Query(None, description="Filter by country"),
    limit: int = Query(50, ge=1, le=100, description="Limit results")
):
    """Get all teams with optional filters"""
    if league:
        teams = await db_manager.get_teams_by_league(league)
    else:
        teams = await db_manager.get_all_teams()
    
    if country:
        teams = [team for team in teams if team.country == country]
    
    return teams[:limit]

@api_router.get("/teams/{team_id}", response_model=Team)
async def get_team_by_id(team_id: str = FastAPIPath(..., description="Team ID")):
    """Get team by ID"""
    team = await db_manager.get_team_by_id(team_id)
    if not team:
        raise HTTPException(status_code=404, detail="Team not found")
    return team

@api_router.get("/teams/{team_id}/players", response_model=List[Player])
async def get_team_players(team_id: str = Path(..., description="Team ID")):
    """Get all players from a team"""
    team = await db_manager.get_team_by_id(team_id)
    if not team:
        raise HTTPException(status_code=404, detail="Team not found")
    return team.players

@api_router.post("/teams/{team_id}/players")
async def add_player_to_team(
    team_id: str,
    player: Player
):
    """Add a player to a team"""
    try:
        await db.teams.update_one(
            {"id": team_id},
            {"$push": {"players": player.dict()}}
        )
        return {"message": "Player added to team successfully"}
    except Exception as e:
        raise HTTPException(status_code=400, detail=str(e))

@api_router.get("/leagues")
async def get_leagues():
    """Get all available leagues"""
    cursor = db.teams.distinct("league")
    leagues = await cursor
    return {"leagues": leagues}

@api_router.get("/countries")
async def get_countries():
    """Get all available countries"""
    cursor = db.teams.distinct("country")
    countries = await cursor
    return {"countries": countries}

# ============ STADIUM ENDPOINTS ============

@api_router.get("/stadiums", response_model=List[Stadium])
async def get_all_stadiums():
    """Get all stadiums"""
    return await db_manager.get_all_stadiums()

@api_router.get("/stadiums/{stadium_id}", response_model=Stadium)
async def get_stadium_by_id(stadium_id: str):
    """Get stadium by ID"""
    result = await db.stadiums.find_one({"id": stadium_id})
    if not result:
        raise HTTPException(status_code=404, detail="Stadium not found")
    return Stadium(**result)

# ============ MATCH ENDPOINTS ============

@api_router.post("/matches", response_model=dict)
async def create_match(match: Match):
    """Create a new match"""
    try:
        match_id = await db_manager.create_match(match.dict())
        return {"match_id": match_id, "message": "Match created successfully"}
    except Exception as e:
        raise HTTPException(status_code=400, detail=str(e))

@api_router.get("/matches/{match_id}", response_model=Match)
async def get_match_by_id(match_id: str):
    """Get match by ID"""
    result = await db.matches.find_one({"id": match_id})
    if not result:
        raise HTTPException(status_code=404, detail="Match not found")
    return Match(**result)

@api_router.get("/users/{user_id}/matches", response_model=List[Match])
async def get_user_matches(user_id: str):
    """Get all matches for a user"""
    return await db_manager.get_user_matches(user_id)

@api_router.put("/matches/{match_id}/complete")
async def complete_match(
    match_id: str,
    match_result: Dict[str, Any]
):
    """Complete a match with results"""
    try:
        await db.matches.update_one(
            {"id": match_id},
            {"$set": {
                "completed": True,
                "home_score": match_result.get("home_score", 0),
                "away_score": match_result.get("away_score", 0),
                "statistics": match_result.get("statistics", {}),
                "match_events": match_result.get("events", [])
            }}
        )
        return {"message": "Match completed successfully"}
    except Exception as e:
        raise HTTPException(status_code=400, detail=str(e))

# ============ TOURNAMENT ENDPOINTS ============

@api_router.post("/tournaments", response_model=dict)
async def create_tournament(tournament: Tournament):
    """Create a new tournament"""
    try:
        tournament_id = await db_manager.create_tournament(tournament.dict())
        return {"tournament_id": tournament_id, "message": "Tournament created successfully"}
    except Exception as e:
        raise HTTPException(status_code=400, detail=str(e))

@api_router.get("/tournaments", response_model=List[Tournament])
async def get_all_tournaments():
    """Get all tournaments"""
    cursor = db.tournaments.find()
    tournaments = []
    async for tournament_doc in cursor:
        tournaments.append(Tournament(**tournament_doc))
    return tournaments

@api_router.get("/tournaments/{tournament_id}", response_model=Tournament)
async def get_tournament_by_id(tournament_id: str):
    """Get tournament by ID"""
    result = await db.tournaments.find_one({"id": tournament_id})
    if not result:
        raise HTTPException(status_code=404, detail="Tournament not found")
    return Tournament(**result)

# ============ CAREER MODE ENDPOINTS ============

@api_router.post("/careers", response_model=dict)
async def create_career(career: Career):
    """Create a new career"""
    try:
        career_id = await db_manager.create_career(career.dict())
        return {"career_id": career_id, "message": "Career created successfully"}
    except Exception as e:
        raise HTTPException(status_code=400, detail=str(e))

@api_router.get("/users/{user_id}/career", response_model=Career)
async def get_user_career(user_id: str):
    """Get user's career"""
    career = await db_manager.get_user_career(user_id)
    if not career:
        raise HTTPException(status_code=404, detail="Career not found")
    return career

@api_router.put("/careers/{career_id}/advance-season")
async def advance_career_season(career_id: str):
    """Advance to next season"""
    try:
        await db.careers.update_one(
            {"id": career_id},
            {"$inc": {"current_season": 1}}
        )
        return {"message": "Season advanced successfully"}
    except Exception as e:
        raise HTTPException(status_code=400, detail=str(e))

# ============ ACHIEVEMENT ENDPOINTS ============

@api_router.get("/achievements", response_model=List[Achievement])
async def get_all_achievements():
    """Get all achievements"""
    return await db_manager.get_all_achievements()

@api_router.get("/users/{user_id}/achievements")
async def get_user_achievements(user_id: str):
    """Get user's unlocked achievements"""
    profile = await db_manager.get_user_profile(user_id)
    if not profile:
        raise HTTPException(status_code=404, detail="User not found")
    
    achieved_ids = profile.achievements
    cursor = db.achievements.find({"id": {"$in": achieved_ids}})
    achievements = []
    async for achievement_doc in cursor:
        achievements.append(Achievement(**achievement_doc))
    
    return achievements

@api_router.post("/users/{user_id}/achievements/{achievement_id}")
async def unlock_achievement(user_id: str, achievement_id: str):
    """Unlock an achievement for a user"""
    try:
        await db_manager.update_user_achievement(user_id, achievement_id)
        return {"message": "Achievement unlocked successfully"}
    except Exception as e:
        raise HTTPException(status_code=400, detail=str(e))

# ============ GAME MODE ENDPOINTS ============

@api_router.get("/game-modes")
async def get_game_modes():
    """Get all available game modes"""
    return {
        "modes": [
            {
                "id": "quick_match",
                "name": "Quick Match",
                "description": "Jump into a quick match with any team",
                "icon": "⚡",
                "max_players": 2,
                "duration": 90
            },
            {
                "id": "career",
                "name": "Career Mode",
                "description": "Build your legacy as a manager",
                "icon": "👔",
                "max_players": 1,
                "duration": 0
            },
            {
                "id": "tournament",
                "name": "Tournament",
                "description": "Compete in various tournaments",
                "icon": "🏆",
                "max_players": 32,
                "duration": 0
            },
            {
                "id": "futsal",
                "name": "Futsal",
                "description": "Fast-paced 5v5 indoor football",
                "icon": "🏟️",
                "max_players": 2,
                "duration": 40
            },
            {
                "id": "online",
                "name": "Online Match",
                "description": "Play against other players online",
                "icon": "🌐",
                "max_players": 2,
                "duration": 90
            }
        ]
    }

# ============ UNIFORM SYSTEM ENDPOINTS ============

@api_router.get("/teams/{team_id}/uniforms", response_model=List[UniformKit])
async def get_team_uniforms(team_id: str):
    """Get team's uniform kits"""
    cursor = db.uniform_kits.find({"team_id": team_id})
    uniforms = []
    async for uniform_doc in cursor:
        uniforms.append(UniformKit(**uniform_doc))
    return uniforms

@api_router.post("/teams/{team_id}/uniforms")
async def create_team_uniform(team_id: str, uniform: UniformKit):
    """Create a new uniform kit for a team"""
    try:
        uniform.team_id = team_id
        await db.uniform_kits.insert_one(uniform.dict())
        return {"message": "Uniform created successfully"}
    except Exception as e:
        raise HTTPException(status_code=400, detail=str(e))

# ============ STATISTICS ENDPOINTS ============

@api_router.get("/users/{user_id}/statistics")
async def get_user_statistics(user_id: str):
    """Get comprehensive user statistics"""
    profile = await db_manager.get_user_profile(user_id)
    if not profile:
        raise HTTPException(status_code=404, detail="User not found")
    
    matches = await db_manager.get_user_matches(user_id)
    
    # Calculate additional stats
    total_goals = sum(match.statistics.get("goals_scored", 0) for match in matches if match.statistics)
    total_assists = sum(match.statistics.get("assists", 0) for match in matches if match.statistics)
    total_cards = sum(match.statistics.get("cards", 0) for match in matches if match.statistics)
    
    win_rate = (profile.total_wins / profile.total_matches * 100) if profile.total_matches > 0 else 0
    
    return {
        "profile": profile,
        "match_statistics": {
            "total_matches": profile.total_matches,
            "wins": profile.total_wins,
            "draws": profile.total_draws,
            "losses": profile.total_losses,
            "win_rate": round(win_rate, 2),
            "goals_scored": profile.total_goals_scored,
            "goals_conceded": profile.total_goals_conceded,
            "goal_difference": profile.total_goals_scored - profile.total_goals_conceded,
            "total_goals": total_goals,
            "total_assists": total_assists,
            "total_cards": total_cards
        },
        "achievements_unlocked": len(profile.achievements),
        "level": profile.level,
        "experience": profile.experience
    }

# ============ SEARCH ENDPOINTS ============

@api_router.get("/search/teams")
async def search_teams(
    query: str = Query(..., description="Search query"),
    limit: int = Query(10, ge=1, le=50)
):
    """Search teams by name or league"""
    cursor = db.teams.find({
        "$or": [
            {"name": {"$regex": query, "$options": "i"}},
            {"league": {"$regex": query, "$options": "i"}},
            {"country": {"$regex": query, "$options": "i"}}
        ]
    }).limit(limit)
    
    teams = []
    async for team_doc in cursor:
        teams.append(Team(**team_doc))
    
    return {"query": query, "results": teams}

@api_router.get("/search/players")
async def search_players(
    query: str = Query(..., description="Search query"),
    position: Optional[Position] = Query(None, description="Filter by position"),
    limit: int = Query(10, ge=1, le=50)
):
    """Search players by name or position"""
    # This would require a different data structure for efficient player search
    # For now, we'll search through all teams
    all_teams = await db_manager.get_all_teams()
    players = []
    
    for team in all_teams:
        for player in team.players:
            if query.lower() in player.name.lower():
                if position is None or player.position == position:
                    players.append({
                        "player": player,
                        "team": {"id": team.id, "name": team.name}
                    })
                    
                    if len(players) >= limit:
                        break
        if len(players) >= limit:
            break
    
    return {"query": query, "results": players}

# Include the router in the main app
app.include_router(api_router)

# Shutdown event
@app.on_event("shutdown")
async def shutdown_db_client():
    client.close()

if __name__ == "__main__":
    import uvicorn
    uvicorn.run(app, host="0.0.0.0", port=8001)
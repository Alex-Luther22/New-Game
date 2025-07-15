import React, { useState, useEffect } from 'react';
import axios from 'axios';

const BACKEND_URL = process.env.REACT_APP_BACKEND_URL;
const API = `${BACKEND_URL}/api`;

const CareerMode = ({ userId }) => {
  const [career, setCareer] = useState(null);
  const [loading, setLoading] = useState(true);
  const [currentView, setCurrentView] = useState('overview');
  const [teams, setTeams] = useState([]);
  const [selectedTeam, setSelectedTeam] = useState(null);

  useEffect(() => {
    fetchCareerData();
    fetchTeams();
  }, [userId]);

  const fetchCareerData = async () => {
    try {
      const response = await axios.get(`${API}/users/${userId}/career`);
      setCareer(response.data);
      setLoading(false);
    } catch (error) {
      if (error.response?.status === 404) {
        // No career found, user needs to create one
        setCareer(null);
        setLoading(false);
      } else {
        console.error('Error fetching career:', error);
        setLoading(false);
      }
    }
  };

  const fetchTeams = async () => {
    try {
      const response = await axios.get(`${API}/teams?limit=50`);
      setTeams(response.data);
    } catch (error) {
      console.error('Error fetching teams:', error);
    }
  };

  const createCareer = async (teamId) => {
    try {
      const contractEndDate = new Date();
      contractEndDate.setFullYear(contractEndDate.getFullYear() + 2);

      const careerData = {
        user_id: userId,
        current_team_id: teamId,
        current_season: 1,
        reputation: 1,
        budget: 5000000,
        objectives: [
          {
            type: "league_position",
            target: 10,
            description: "Finish in top 10 of the league",
            reward: 1000000,
            completed: false
          },
          {
            type: "cup_progress",
            target: "quarter_finals",
            description: "Reach quarter-finals in domestic cup",
            reward: 500000,
            completed: false
          }
        ],
        contract_end_date: contractEndDate.toISOString(),
        season_stats: {
          matches_played: 0,
          wins: 0,
          draws: 0,
          losses: 0,
          goals_for: 0,
          goals_against: 0,
          league_position: 0
        },
        transfer_history: []
      };

      const response = await axios.post(`${API}/careers`, careerData);
      if (response.data.career_id) {
        fetchCareerData();
      }
    } catch (error) {
      console.error('Error creating career:', error);
    }
  };

  const advanceSeason = async () => {
    try {
      await axios.put(`${API}/careers/${career.id}/advance-season`);
      fetchCareerData();
    } catch (error) {
      console.error('Error advancing season:', error);
    }
  };

  const renderTeamSelection = () => (
    <div className="bg-white rounded-xl shadow-lg p-8">
      <h2 className="text-3xl font-bold text-center mb-8 text-gray-800">
        👔 Comienza tu Carrera de Entrenador
      </h2>
      
      <div className="mb-6 text-center">
        <p className="text-lg text-gray-600">
          Selecciona el equipo que quieres entrenar y comienza tu carrera hacia la gloria
        </p>
      </div>
      
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4 max-h-96 overflow-y-auto">
        {teams.map((team) => (
          <div
            key={team.id}
            className={`p-4 rounded-lg border-2 cursor-pointer transition-all duration-300 hover:scale-105 ${
              selectedTeam?.id === team.id
                ? 'border-green-500 bg-green-50 shadow-xl'
                : 'border-gray-200 hover:border-green-300 hover:shadow-lg'
            }`}
            onClick={() => setSelectedTeam(team)}
          >
            <div className="text-center">
              <div className="flex justify-center mb-3">
                <div className="flex space-x-1">
                  <div 
                    className="w-6 h-6 rounded-full border-2 border-white shadow-sm"
                    style={{ backgroundColor: team.primary_color }}
                  ></div>
                  <div 
                    className="w-6 h-6 rounded-full border-2 border-white shadow-sm"
                    style={{ backgroundColor: team.secondary_color }}
                  ></div>
                </div>
              </div>
              
              <h3 className="text-lg font-semibold text-gray-800 mb-1">
                {team.name}
              </h3>
              <p className="text-sm text-gray-600 mb-2">
                {team.league} | {team.country}
              </p>
              
              <div className="mb-3">
                <span className="inline-block px-3 py-1 rounded-full text-sm font-semibold bg-blue-100 text-blue-800">
                  {team.overall_rating} OVR
                </span>
              </div>
              
              <div className="text-xs text-gray-500">
                💰 Presupuesto: ${team.budget.toLocaleString()}
              </div>
              <div className="text-xs text-gray-500">
                ⭐ Prestigio: {team.prestige}/10
              </div>
            </div>
          </div>
        ))}
      </div>
      
      {selectedTeam && (
        <div className="mt-8 p-6 bg-gradient-to-r from-green-500 to-blue-500 rounded-lg text-white">
          <h3 className="text-xl font-bold mb-4">
            Únete al {selectedTeam.name}
          </h3>
          <div className="grid grid-cols-2 gap-4 mb-4">
            <div>
              <p className="text-sm opacity-90">Liga: {selectedTeam.league}</p>
              <p className="text-sm opacity-90">País: {selectedTeam.country}</p>
              <p className="text-sm opacity-90">Prestigio: {selectedTeam.prestige}/10</p>
            </div>
            <div>
              <p className="text-sm opacity-90">Presupuesto: ${selectedTeam.budget.toLocaleString()}</p>
              <p className="text-sm opacity-90">Estadio: {selectedTeam.stadium_name}</p>
              <p className="text-sm opacity-90">Capacidad: {selectedTeam.stadium_capacity.toLocaleString()}</p>
            </div>
          </div>
          
          <button
            onClick={() => createCareer(selectedTeam.id)}
            className="w-full bg-white text-green-600 px-6 py-3 rounded-full font-semibold hover:bg-gray-100 transition-colors"
          >
            Comenzar Carrera
          </button>
        </div>
      )}
    </div>
  );

  const renderCareerOverview = () => (
    <div className="space-y-6">
      {/* Header */}
      <div className="bg-gradient-to-r from-green-500 to-blue-500 rounded-xl p-6 text-white">
        <div className="flex justify-between items-center">
          <div>
            <h2 className="text-2xl font-bold">Carrera de Entrenador</h2>
            <p className="text-lg opacity-90">Temporada {career.current_season}</p>
          </div>
          <div className="text-right">
            <div className="text-3xl font-bold">⭐ {career.reputation}/10</div>
            <div className="text-sm opacity-90">Reputación</div>
          </div>
        </div>
      </div>
      
      {/* Stats Cards */}
      <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
        <div className="bg-white rounded-xl shadow-lg p-6">
          <div className="flex items-center justify-between">
            <div>
              <h3 className="text-lg font-semibold text-gray-800">Presupuesto</h3>
              <p className="text-2xl font-bold text-green-600">
                ${career.budget.toLocaleString()}
              </p>
            </div>
            <div className="text-4xl">💰</div>
          </div>
        </div>
        
        <div className="bg-white rounded-xl shadow-lg p-6">
          <div className="flex items-center justify-between">
            <div>
              <h3 className="text-lg font-semibold text-gray-800">Estadísticas</h3>
              <p className="text-sm text-gray-600">
                PJ: {career.season_stats.matches_played} | 
                G: {career.season_stats.wins} | 
                E: {career.season_stats.draws} | 
                P: {career.season_stats.losses}
              </p>
            </div>
            <div className="text-4xl">📊</div>
          </div>
        </div>
        
        <div className="bg-white rounded-xl shadow-lg p-6">
          <div className="flex items-center justify-between">
            <div>
              <h3 className="text-lg font-semibold text-gray-800">Posición Liga</h3>
              <p className="text-2xl font-bold text-blue-600">
                {career.season_stats.league_position || 'N/A'}
              </p>
            </div>
            <div className="text-4xl">🏆</div>
          </div>
        </div>
      </div>
      
      {/* Objectives */}
      <div className="bg-white rounded-xl shadow-lg p-6">
        <h3 className="text-xl font-bold text-gray-800 mb-4">🎯 Objetivos de Temporada</h3>
        <div className="space-y-3">
          {career.objectives.map((objective, index) => (
            <div key={index} className={`p-4 rounded-lg border-2 ${
              objective.completed ? 'border-green-500 bg-green-50' : 'border-gray-200'
            }`}>
              <div className="flex justify-between items-center">
                <div>
                  <h4 className="font-semibold text-gray-800">{objective.description}</h4>
                  <p className="text-sm text-gray-600">
                    Recompensa: ${objective.reward.toLocaleString()}
                  </p>
                </div>
                <div className="text-2xl">
                  {objective.completed ? '✅' : '⏳'}
                </div>
              </div>
            </div>
          ))}
        </div>
      </div>
      
      {/* Actions */}
      <div className="bg-white rounded-xl shadow-lg p-6">
        <h3 className="text-xl font-bold text-gray-800 mb-4">🎮 Acciones</h3>
        <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
          <button
            onClick={() => setCurrentView('transfers')}
            className="bg-blue-500 hover:bg-blue-600 text-white px-6 py-3 rounded-lg font-semibold transition-colors"
          >
            🔄 Transferencias
          </button>
          <button
            onClick={() => setCurrentView('matches')}
            className="bg-green-500 hover:bg-green-600 text-white px-6 py-3 rounded-lg font-semibold transition-colors"
          >
            ⚽ Jugar Partido
          </button>
          <button
            onClick={() => setCurrentView('team')}
            className="bg-purple-500 hover:bg-purple-600 text-white px-6 py-3 rounded-lg font-semibold transition-colors"
          >
            👥 Gestionar Equipo
          </button>
          <button
            onClick={advanceSeason}
            className="bg-orange-500 hover:bg-orange-600 text-white px-6 py-3 rounded-lg font-semibold transition-colors"
          >
            ⏭️ Avanzar Temporada
          </button>
        </div>
      </div>
    </div>
  );

  const renderNavigationTabs = () => (
    <div className="flex space-x-4 mb-6">
      {[
        { key: 'overview', label: '📊 Resumen', icon: '📊' },
        { key: 'transfers', label: '🔄 Transferencias', icon: '🔄' },
        { key: 'matches', label: '⚽ Partidos', icon: '⚽' },
        { key: 'team', label: '👥 Equipo', icon: '👥' }
      ].map((tab) => (
        <button
          key={tab.key}
          onClick={() => setCurrentView(tab.key)}
          className={`px-4 py-2 rounded-lg font-semibold transition-colors ${
            currentView === tab.key
              ? 'bg-green-500 text-white'
              : 'bg-gray-200 text-gray-700 hover:bg-gray-300'
          }`}
        >
          {tab.label}
        </button>
      ))}
    </div>
  );

  if (loading) {
    return (
      <div className="flex justify-center items-center h-64">
        <div className="animate-spin rounded-full h-16 w-16 border-b-2 border-green-600"></div>
      </div>
    );
  }

  if (!career) {
    return renderTeamSelection();
  }

  return (
    <div className="max-w-7xl mx-auto p-6">
      {renderNavigationTabs()}
      
      {currentView === 'overview' && renderCareerOverview()}
      {currentView === 'transfers' && (
        <div className="bg-white rounded-xl shadow-lg p-8">
          <h2 className="text-2xl font-bold text-gray-800 mb-6">🔄 Transferencias</h2>
          <p className="text-gray-600">Sistema de transferencias en desarrollo...</p>
        </div>
      )}
      {currentView === 'matches' && (
        <div className="bg-white rounded-xl shadow-lg p-8">
          <h2 className="text-2xl font-bold text-gray-800 mb-6">⚽ Partidos</h2>
          <p className="text-gray-600">Calendario de partidos en desarrollo...</p>
        </div>
      )}
      {currentView === 'team' && (
        <div className="bg-white rounded-xl shadow-lg p-8">
          <h2 className="text-2xl font-bold text-gray-800 mb-6">👥 Gestión de Equipo</h2>
          <p className="text-gray-600">Gestión de equipo en desarrollo...</p>
        </div>
      )}
    </div>
  );
};

export default CareerMode;
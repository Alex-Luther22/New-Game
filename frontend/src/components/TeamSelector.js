import React, { useState, useEffect } from 'react';
import axios from 'axios';

const BACKEND_URL = process.env.REACT_APP_BACKEND_URL;
const API = `${BACKEND_URL}/api`;

const TeamSelector = ({ onTeamSelect, gameMode }) => {
  const [teams, setTeams] = useState([]);
  const [filteredTeams, setFilteredTeams] = useState([]);
  const [selectedTeam, setSelectedTeam] = useState(null);
  const [loading, setLoading] = useState(true);
  const [searchQuery, setSearchQuery] = useState('');
  const [selectedLeague, setSelectedLeague] = useState('');
  const [selectedCountry, setSelectedCountry] = useState('');
  const [leagues, setLeagues] = useState([]);
  const [countries, setCountries] = useState([]);

  useEffect(() => {
    fetchTeams();
    fetchLeagues();
    fetchCountries();
  }, []);

  useEffect(() => {
    filterTeams();
  }, [teams, searchQuery, selectedLeague, selectedCountry]);

  const fetchTeams = async () => {
    try {
      const response = await axios.get(`${API}/teams?limit=100`);
      setTeams(response.data);
      setFilteredTeams(response.data);
      setLoading(false);
    } catch (error) {
      console.error('Error fetching teams:', error);
      setLoading(false);
    }
  };

  const fetchLeagues = async () => {
    try {
      const response = await axios.get(`${API}/leagues`);
      setLeagues(response.data.leagues);
    } catch (error) {
      console.error('Error fetching leagues:', error);
    }
  };

  const fetchCountries = async () => {
    try {
      const response = await axios.get(`${API}/countries`);
      setCountries(response.data.countries);
    } catch (error) {
      console.error('Error fetching countries:', error);
    }
  };

  const filterTeams = () => {
    let filtered = teams;

    if (searchQuery) {
      filtered = filtered.filter(team => 
        team.name.toLowerCase().includes(searchQuery.toLowerCase()) ||
        team.short_name.toLowerCase().includes(searchQuery.toLowerCase())
      );
    }

    if (selectedLeague) {
      filtered = filtered.filter(team => team.league === selectedLeague);
    }

    if (selectedCountry) {
      filtered = filtered.filter(team => team.country === selectedCountry);
    }

    setFilteredTeams(filtered);
  };

  const handleTeamSelect = (team) => {
    setSelectedTeam(team);
    onTeamSelect(team);
  };

  const getTeamRatingColor = (rating) => {
    if (rating >= 85) return 'text-green-600';
    if (rating >= 75) return 'text-yellow-600';
    if (rating >= 65) return 'text-orange-600';
    return 'text-red-600';
  };

  const getTeamRatingBadge = (rating) => {
    if (rating >= 85) return 'bg-green-100 text-green-800';
    if (rating >= 75) return 'bg-yellow-100 text-yellow-800';
    if (rating >= 65) return 'bg-orange-100 text-orange-800';
    return 'bg-red-100 text-red-800';
  };

  if (loading) {
    return (
      <div className="flex justify-center items-center h-64">
        <div className="animate-spin rounded-full h-16 w-16 border-b-2 border-green-600"></div>
      </div>
    );
  }

  return (
    <div className="bg-white rounded-xl shadow-lg p-8">
      <h2 className="text-3xl font-bold text-center mb-8 text-gray-800">
        ⚽ Selecciona tu Equipo
      </h2>
      
      {/* Filters */}
      <div className="mb-6 grid grid-cols-1 md:grid-cols-3 gap-4">
        <div>
          <label className="block text-sm font-medium text-gray-700 mb-2">
            🔍 Buscar Equipo
          </label>
          <input
            type="text"
            value={searchQuery}
            onChange={(e) => setSearchQuery(e.target.value)}
            placeholder="Nombre del equipo..."
            className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-green-500 focus:border-transparent"
          />
        </div>
        
        <div>
          <label className="block text-sm font-medium text-gray-700 mb-2">
            🏆 Liga
          </label>
          <select
            value={selectedLeague}
            onChange={(e) => setSelectedLeague(e.target.value)}
            className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-green-500 focus:border-transparent"
          >
            <option value="">Todas las ligas</option>
            {leagues.map((league) => (
              <option key={league} value={league}>
                {league}
              </option>
            ))}
          </select>
        </div>
        
        <div>
          <label className="block text-sm font-medium text-gray-700 mb-2">
            🌍 País
          </label>
          <select
            value={selectedCountry}
            onChange={(e) => setSelectedCountry(e.target.value)}
            className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-green-500 focus:border-transparent"
          >
            <option value="">Todos los países</option>
            {countries.map((country) => (
              <option key={country} value={country}>
                {country}
              </option>
            ))}
          </select>
        </div>
      </div>
      
      {/* Team Grid */}
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4 max-h-96 overflow-y-auto">
        {filteredTeams.map((team) => (
          <div
            key={team.id}
            className={`p-4 rounded-lg border-2 cursor-pointer transition-all duration-300 hover:scale-105 ${
              selectedTeam?.id === team.id
                ? 'border-green-500 bg-green-50 shadow-xl'
                : 'border-gray-200 hover:border-green-300 hover:shadow-lg'
            }`}
            onClick={() => handleTeamSelect(team)}
          >
            <div className="text-center">
              {/* Team Colors */}
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
                {team.short_name} | {team.league}
              </p>
              
              {/* Overall Rating */}
              <div className="mb-3">
                <span className={`inline-block px-3 py-1 rounded-full text-sm font-semibold ${getTeamRatingBadge(team.overall_rating)}`}>
                  {team.overall_rating} OVR
                </span>
              </div>
              
              {/* Stats */}
              <div className="grid grid-cols-3 gap-2 text-xs">
                <div className="text-center">
                  <div className={`font-semibold ${getTeamRatingColor(team.attack_rating)}`}>
                    {team.attack_rating}
                  </div>
                  <div className="text-gray-500">ATT</div>
                </div>
                <div className="text-center">
                  <div className={`font-semibold ${getTeamRatingColor(team.midfield_rating)}`}>
                    {team.midfield_rating}
                  </div>
                  <div className="text-gray-500">MID</div>
                </div>
                <div className="text-center">
                  <div className={`font-semibold ${getTeamRatingColor(team.defense_rating)}`}>
                    {team.defense_rating}
                  </div>
                  <div className="text-gray-500">DEF</div>
                </div>
              </div>
              
              {/* Stadium */}
              <div className="mt-2 text-xs text-gray-500">
                🏟️ {team.stadium_name}
              </div>
              
              {selectedTeam?.id === team.id && (
                <div className="mt-3 p-2 bg-green-100 rounded-lg">
                  <span className="text-green-800 font-medium text-sm">✓ Seleccionado</span>
                </div>
              )}
            </div>
          </div>
        ))}
      </div>
      
      {filteredTeams.length === 0 && (
        <div className="text-center py-8">
          <p className="text-gray-500">No se encontraron equipos con los filtros seleccionados.</p>
        </div>
      )}
      
      {selectedTeam && (
        <div className="mt-8 p-6 bg-gradient-to-r from-green-500 to-blue-500 rounded-lg text-white">
          <div className="flex items-center justify-between">
            <div>
              <h3 className="text-xl font-bold mb-2">
                {selectedTeam.name} ({selectedTeam.short_name})
              </h3>
              <p className="mb-2">{selectedTeam.league} | {selectedTeam.country}</p>
              <p className="text-sm opacity-90">
                🏟️ {selectedTeam.stadium_name} | Capacidad: {selectedTeam.stadium_capacity.toLocaleString()}
              </p>
            </div>
            <div className="text-right">
              <div className="text-3xl font-bold mb-2">
                {selectedTeam.overall_rating}
              </div>
              <div className="text-sm opacity-90">Rating General</div>
            </div>
          </div>
          
          <div className="mt-4 flex justify-between items-center">
            <div>
              <span className="text-sm opacity-90">Equipo seleccionado</span>
            </div>
            <button
              onClick={() => onTeamSelect(selectedTeam)}
              className="bg-white text-green-600 px-6 py-2 rounded-full font-semibold hover:bg-gray-100 transition-colors"
            >
              Continuar
            </button>
          </div>
        </div>
      )}
    </div>
  );
};

export default TeamSelector;
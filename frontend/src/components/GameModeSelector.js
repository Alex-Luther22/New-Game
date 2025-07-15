import React, { useState, useEffect } from 'react';
import axios from 'axios';

const BACKEND_URL = process.env.REACT_APP_BACKEND_URL;
const API = `${BACKEND_URL}/api`;

const GameModeSelector = ({ onModeSelect }) => {
  const [gameModes, setGameModes] = useState([]);
  const [selectedMode, setSelectedMode] = useState(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    fetchGameModes();
  }, []);

  const fetchGameModes = async () => {
    try {
      const response = await axios.get(`${API}/game-modes`);
      setGameModes(response.data.modes);
      setLoading(false);
    } catch (error) {
      console.error('Error fetching game modes:', error);
      setLoading(false);
    }
  };

  const handleModeSelect = (mode) => {
    setSelectedMode(mode);
    onModeSelect(mode);
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
        🎮 Selecciona Modo de Juego
      </h2>
      
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
        {gameModes.map((mode) => (
          <div
            key={mode.id}
            className={`p-6 rounded-lg border-2 cursor-pointer transition-all duration-300 hover:scale-105 ${
              selectedMode?.id === mode.id
                ? 'border-green-500 bg-green-50 shadow-xl'
                : 'border-gray-200 hover:border-green-300 hover:shadow-lg'
            }`}
            onClick={() => handleModeSelect(mode)}
          >
            <div className="text-center">
              <div className="text-4xl mb-4">{mode.icon}</div>
              <h3 className="text-xl font-semibold text-gray-800 mb-2">
                {mode.name}
              </h3>
              <p className="text-gray-600 text-sm mb-4">
                {mode.description}
              </p>
              
              <div className="space-y-2">
                <div className="flex justify-between text-sm">
                  <span className="text-gray-500">Jugadores:</span>
                  <span className="font-medium">{mode.max_players}</span>
                </div>
                {mode.duration > 0 && (
                  <div className="flex justify-between text-sm">
                    <span className="text-gray-500">Duración:</span>
                    <span className="font-medium">{mode.duration} min</span>
                  </div>
                )}
              </div>
              
              {selectedMode?.id === mode.id && (
                <div className="mt-4 p-2 bg-green-100 rounded-lg">
                  <span className="text-green-800 font-medium">✓ Seleccionado</span>
                </div>
              )}
            </div>
          </div>
        ))}
      </div>
      
      {selectedMode && (
        <div className="mt-8 p-6 bg-gradient-to-r from-green-500 to-blue-500 rounded-lg text-white">
          <h3 className="text-xl font-bold mb-2">
            {selectedMode.icon} {selectedMode.name}
          </h3>
          <p className="mb-4">{selectedMode.description}</p>
          <div className="flex justify-between items-center">
            <div>
              <span className="text-sm opacity-90">Modo seleccionado</span>
            </div>
            <button
              onClick={() => onModeSelect(selectedMode)}
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

export default GameModeSelector;
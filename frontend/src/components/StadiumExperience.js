import React, { useState, useEffect } from 'react';
import axios from 'axios';

const BACKEND_URL = process.env.REACT_APP_BACKEND_URL;
const API = `${BACKEND_URL}/api`;

const StadiumExperience = () => {
  const [stadiums, setStadiums] = useState([]);
  const [selectedStadium, setSelectedStadium] = useState(null);
  const [loading, setLoading] = useState(true);
  const [currentView, setCurrentView] = useState('gallery');

  useEffect(() => {
    fetchStadiums();
  }, []);

  const fetchStadiums = async () => {
    try {
      const response = await axios.get(`${API}/stadiums`);
      setStadiums(response.data);
      setLoading(false);
    } catch (error) {
      console.error('Error fetching stadiums:', error);
      setLoading(false);
    }
  };

  const getWeatherIcon = (weather) => {
    const icons = {
      'sunny': '☀️',
      'cloudy': '☁️',
      'rainy': '🌧️',
      'windy': '💨',
      'snowy': '❄️',
      'hot': '🔥',
      'cold': '🧊',
      'foggy': '🌫️',
      'mild': '🌤️',
      'humid': '💦'
    };
    return icons[weather] || '🌤️';
  };

  const getAtmosphereStars = (rating) => {
    return '⭐'.repeat(rating);
  };

  const getSurfaceColor = (surface) => {
    const colors = {
      'natural_grass': 'bg-green-100 text-green-800',
      'hybrid_grass': 'bg-green-200 text-green-900',
      'artificial_turf': 'bg-blue-100 text-blue-800'
    };
    return colors[surface] || 'bg-gray-100 text-gray-800';
  };

  const getRoofIcon = (roof) => {
    const icons = {
      'open': '🌌',
      'closed': '🏢',
      'retractable': '🔄',
      'partial': '🏟️'
    };
    return icons[roof] || '🏟️';
  };

  const renderStadiumGallery = () => (
    <div className="space-y-6">
      <div className="text-center mb-8">
        <h2 className="text-4xl font-bold text-gray-800 mb-4">
          🏟️ Estadios Únicos del Mundo
        </h2>
        <p className="text-lg text-gray-600">
          Experimenta la magia de los estadios más icónicos del fútbol mundial
        </p>
      </div>
      
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
        {stadiums.map((stadium) => (
          <div
            key={stadium.id}
            className="bg-white rounded-xl shadow-lg overflow-hidden cursor-pointer transform transition-all duration-300 hover:scale-105 hover:shadow-2xl"
            onClick={() => setSelectedStadium(stadium)}
          >
            {/* Stadium Header */}
            <div className="bg-gradient-to-r from-green-500 to-blue-500 p-4 text-white">
              <h3 className="text-xl font-bold mb-2">{stadium.name}</h3>
              <div className="flex justify-between items-center">
                <p className="text-sm opacity-90">
                  {stadium.city}, {stadium.country}
                </p>
                <div className="text-2xl">
                  {getAtmosphereStars(stadium.atmosphere_rating)}
                </div>
              </div>
            </div>
            
            {/* Stadium Info */}
            <div className="p-4">
              <div className="grid grid-cols-2 gap-4 mb-4">
                <div>
                  <p className="text-sm text-gray-600">Capacidad</p>
                  <p className="text-lg font-semibold text-gray-800">
                    {stadium.capacity.toLocaleString()}
                  </p>
                </div>
                <div>
                  <p className="text-sm text-gray-600">Ambiente</p>
                  <p className="text-lg font-semibold text-gray-800">
                    {stadium.atmosphere_rating}/10
                  </p>
                </div>
              </div>
              
              <div className="space-y-2">
                <div className="flex items-center justify-between">
                  <span className="text-sm text-gray-600">Superficie:</span>
                  <span className={`px-2 py-1 rounded-full text-xs font-medium ${getSurfaceColor(stadium.surface_type)}`}>
                    {stadium.surface_type}
                  </span>
                </div>
                
                <div className="flex items-center justify-between">
                  <span className="text-sm text-gray-600">Techo:</span>
                  <span className="text-sm font-medium text-gray-800">
                    {getRoofIcon(stadium.roof_type)} {stadium.roof_type}
                  </span>
                </div>
              </div>
              
              {/* Weather Conditions */}
              <div className="mt-4">
                <p className="text-sm text-gray-600 mb-2">Condiciones climáticas:</p>
                <div className="flex space-x-2">
                  {stadium.weather_conditions.map((weather) => (
                    <span
                      key={weather}
                      className="inline-block px-2 py-1 bg-blue-100 text-blue-800 rounded-full text-xs"
                    >
                      {getWeatherIcon(weather)} {weather}
                    </span>
                  ))}
                </div>
              </div>
              
              {/* Unique Features */}
              {stadium.unique_features && stadium.unique_features.length > 0 && (
                <div className="mt-4">
                  <p className="text-sm text-gray-600 mb-2">Características únicas:</p>
                  <div className="flex flex-wrap gap-1">
                    {stadium.unique_features.map((feature, index) => (
                      <span
                        key={index}
                        className="inline-block px-2 py-1 bg-purple-100 text-purple-800 rounded-full text-xs"
                      >
                        ✨ {feature}
                      </span>
                    ))}
                  </div>
                </div>
              )}
            </div>
          </div>
        ))}
      </div>
    </div>
  );

  const renderStadiumDetail = () => (
    <div className="bg-white rounded-xl shadow-lg overflow-hidden">
      <button
        onClick={() => setSelectedStadium(null)}
        className="m-4 bg-gray-500 hover:bg-gray-600 text-white px-4 py-2 rounded-lg transition-colors"
      >
        ← Volver a la galería
      </button>
      
      {/* Stadium Hero */}
      <div className="bg-gradient-to-r from-green-500 to-blue-500 p-8 text-white">
        <div className="max-w-4xl mx-auto">
          <h1 className="text-4xl font-bold mb-4">{selectedStadium.name}</h1>
          <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
            <div>
              <p className="text-lg opacity-90 mb-2">
                📍 {selectedStadium.city}, {selectedStadium.country}
              </p>
              <p className="text-sm opacity-75">
                Una experiencia única en el fútbol mundial
              </p>
            </div>
            <div className="text-center">
              <div className="text-3xl font-bold mb-1">
                {selectedStadium.capacity.toLocaleString()}
              </div>
              <div className="text-sm opacity-90">Capacidad</div>
            </div>
            <div className="text-center">
              <div className="text-3xl font-bold mb-1">
                {selectedStadium.atmosphere_rating}/10
              </div>
              <div className="text-sm opacity-90">Ambiente</div>
            </div>
          </div>
        </div>
      </div>
      
      {/* Stadium Details */}
      <div className="p-8">
        <div className="max-w-4xl mx-auto">
          <div className="grid grid-cols-1 md:grid-cols-2 gap-8">
            {/* Technical Specifications */}
            <div>
              <h3 className="text-2xl font-bold text-gray-800 mb-4">
                🏗️ Especificaciones Técnicas
              </h3>
              <div className="space-y-4">
                <div className="bg-gray-50 p-4 rounded-lg">
                  <div className="flex justify-between items-center">
                    <span className="font-semibold text-gray-700">Superficie de juego:</span>
                    <span className={`px-3 py-1 rounded-full text-sm font-medium ${getSurfaceColor(selectedStadium.surface_type)}`}>
                      {selectedStadium.surface_type}
                    </span>
                  </div>
                </div>
                
                <div className="bg-gray-50 p-4 rounded-lg">
                  <div className="flex justify-between items-center">
                    <span className="font-semibold text-gray-700">Tipo de techo:</span>
                    <span className="text-gray-800">
                      {getRoofIcon(selectedStadium.roof_type)} {selectedStadium.roof_type}
                    </span>
                  </div>
                </div>
                
                <div className="bg-gray-50 p-4 rounded-lg">
                  <div className="flex justify-between items-center">
                    <span className="font-semibold text-gray-700">Capacidad total:</span>
                    <span className="text-gray-800 font-bold">
                      {selectedStadium.capacity.toLocaleString()} personas
                    </span>
                  </div>
                </div>
                
                <div className="bg-gray-50 p-4 rounded-lg">
                  <div className="flex justify-between items-center">
                    <span className="font-semibold text-gray-700">Rating de ambiente:</span>
                    <div className="flex items-center space-x-2">
                      <span className="text-xl">{getAtmosphereStars(selectedStadium.atmosphere_rating)}</span>
                      <span className="text-gray-800 font-bold">{selectedStadium.atmosphere_rating}/10</span>
                    </div>
                  </div>
                </div>
              </div>
            </div>
            
            {/* Atmosphere & Experience */}
            <div>
              <h3 className="text-2xl font-bold text-gray-800 mb-4">
                🎭 Ambiente y Experiencia
              </h3>
              
              {/* Weather Conditions */}
              <div className="mb-6">
                <h4 className="font-semibold text-gray-700 mb-3">Condiciones climáticas típicas:</h4>
                <div className="grid grid-cols-2 gap-2">
                  {selectedStadium.weather_conditions.map((weather) => (
                    <div
                      key={weather}
                      className="bg-blue-50 p-3 rounded-lg text-center"
                    >
                      <div className="text-2xl mb-1">{getWeatherIcon(weather)}</div>
                      <div className="text-sm font-medium text-blue-800 capitalize">
                        {weather}
                      </div>
                    </div>
                  ))}
                </div>
              </div>
              
              {/* Unique Features */}
              {selectedStadium.unique_features && selectedStadium.unique_features.length > 0 && (
                <div>
                  <h4 className="font-semibold text-gray-700 mb-3">Características únicas:</h4>
                  <div className="space-y-2">
                    {selectedStadium.unique_features.map((feature, index) => (
                      <div
                        key={index}
                        className="bg-purple-50 p-3 rounded-lg flex items-center space-x-3"
                      >
                        <span className="text-purple-600 text-xl">✨</span>
                        <span className="text-purple-800 font-medium capitalize">
                          {feature.replace(/_/g, ' ')}
                        </span>
                      </div>
                    ))}
                  </div>
                </div>
              )}
            </div>
          </div>
          
          {/* Match Experience Simulation */}
          <div className="mt-8 bg-gradient-to-r from-green-50 to-blue-50 p-6 rounded-lg">
            <h3 className="text-2xl font-bold text-gray-800 mb-4">
              🎮 Experiencia de Partido
            </h3>
            <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
              <div className="text-center">
                <div className="text-3xl mb-2">👥</div>
                <div className="font-semibold text-gray-700">Capacidad</div>
                <div className="text-sm text-gray-600">
                  {selectedStadium.capacity.toLocaleString()} aficionados
                </div>
              </div>
              <div className="text-center">
                <div className="text-3xl mb-2">🔥</div>
                <div className="font-semibold text-gray-700">Intensidad</div>
                <div className="text-sm text-gray-600">
                  {selectedStadium.atmosphere_rating >= 8 ? 'Muy alta' : 
                   selectedStadium.atmosphere_rating >= 6 ? 'Alta' : 'Media'}
                </div>
              </div>
              <div className="text-center">
                <div className="text-3xl mb-2">🎵</div>
                <div className="font-semibold text-gray-700">Ambiente</div>
                <div className="text-sm text-gray-600">
                  {selectedStadium.atmosphere_rating >= 9 ? 'Legendario' :
                   selectedStadium.atmosphere_rating >= 7 ? 'Excepcional' : 'Bueno'}
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  );

  if (loading) {
    return (
      <div className="flex justify-center items-center h-64">
        <div className="animate-spin rounded-full h-16 w-16 border-b-2 border-green-600"></div>
      </div>
    );
  }

  return (
    <div className="max-w-7xl mx-auto p-6">
      {selectedStadium ? renderStadiumDetail() : renderStadiumGallery()}
    </div>
  );
};

export default StadiumExperience;
import { useEffect } from "react";
import "./App.css";
import { BrowserRouter, Routes, Route, Link } from "react-router-dom";
import axios from "axios";
import TouchControlsDemo from "./components/TouchControlsDemo";

const BACKEND_URL = process.env.REACT_APP_BACKEND_URL;
const API = `${BACKEND_URL}/api`;

const Home = () => {
  const helloWorldApi = async () => {
    try {
      const response = await axios.get(`${API}/`);
      console.log(response.data.message);
    } catch (e) {
      console.error(e, `errored out requesting / api`);
    }
  };

  useEffect(() => {
    helloWorldApi();
  }, []);

  return (
    <div className="min-h-screen bg-gradient-to-br from-green-100 to-blue-100">
      <div className="container mx-auto px-4 py-8">
        <header className="text-center mb-12">
          <div className="flex justify-center items-center mb-6">
            <img 
              src="https://avatars.githubusercontent.com/in/1201222?s=120&u=2686cf91179bbafbc7a71bfbc43004cf9ae1acea&v=4"
              alt="Logo"
              className="w-20 h-20 rounded-full shadow-lg"
            />
          </div>
          <h1 className="text-5xl font-bold text-gray-800 mb-4">
            ⚽ Football Master
          </h1>
          <p className="text-xl text-gray-600 mb-8">
            Juego de Fútbol Móvil con Controles Táctiles Avanzados
          </p>
          
          <div className="flex flex-col sm:flex-row gap-4 justify-center">
            <Link
              to="/controles"
              className="bg-gradient-to-r from-green-500 to-green-600 text-white px-8 py-4 rounded-full font-semibold text-lg hover:from-green-600 hover:to-green-700 transition-all duration-300 shadow-lg hover:shadow-xl transform hover:-translate-y-1"
            >
              🎮 Probar Controles Interactivos
            </Link>
            <a
              href="https://emergent.sh"
              target="_blank"
              rel="noopener noreferrer"
              className="bg-gradient-to-r from-blue-500 to-blue-600 text-white px-8 py-4 rounded-full font-semibold text-lg hover:from-blue-600 hover:to-blue-700 transition-all duration-300 shadow-lg hover:shadow-xl transform hover:-translate-y-1"
            >
              🚀 Powered by Emergent
            </a>
          </div>
        </header>
        
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-8 mb-12">
          <div className="bg-white rounded-xl shadow-lg p-6 hover:shadow-xl transition-shadow duration-300">
            <div className="text-4xl mb-4">⚽</div>
            <h3 className="text-xl font-semibold text-gray-800 mb-2">Física Realista</h3>
            <p className="text-gray-600">
              Efecto Magnus, curvas auténticas, rebotes realistas y fricción del césped.
            </p>
          </div>
          
          <div className="bg-white rounded-xl shadow-lg p-6 hover:shadow-xl transition-shadow duration-300">
            <div className="text-4xl mb-4">🎮</div>
            <h3 className="text-xl font-semibold text-gray-800 mb-2">Controles Táctiles</h3>
            <p className="text-gray-600">
              8 trucos diferentes: Roulette, Elastico, Step-over, Nutmeg y más.
            </p>
          </div>
          
          <div className="bg-white rounded-xl shadow-lg p-6 hover:shadow-xl transition-shadow duration-300">
            <div className="text-4xl mb-4">🤖</div>
            <h3 className="text-xl font-semibold text-gray-800 mb-2">IA Avanzada</h3>
            <p className="text-gray-600">
              Jugadores inteligentes con estados de IA realistas y comportamiento auténtico.
            </p>
          </div>
          
          <div className="bg-white rounded-xl shadow-lg p-6 hover:shadow-xl transition-shadow duration-300">
            <div className="text-4xl mb-4">📱</div>
            <h3 className="text-xl font-semibold text-gray-800 mb-2">Optimizado Móvil</h3>
            <p className="text-gray-600">
              Funciona perfectamente en dispositivos como Tecno Spark 8C y superiores.
            </p>
          </div>
          
          <div className="bg-white rounded-xl shadow-lg p-6 hover:shadow-xl transition-shadow duration-300">
            <div className="text-4xl mb-4">🏆</div>
            <h3 className="text-xl font-semibold text-gray-800 mb-2">Modos de Juego</h3>
            <p className="text-gray-600">
              Todos los modos de FIFA 2025 incluyendo Futsal, Carrera, Torneos y más.
            </p>
          </div>
          
          <div className="bg-white rounded-xl shadow-lg p-6 hover:shadow-xl transition-shadow duration-300">
            <div className="text-4xl mb-4">🌐</div>
            <h3 className="text-xl font-semibold text-gray-800 mb-2">Ranking Mundial</h3>
            <p className="text-gray-600">
              Competencia global con estadísticas online y clasificaciones mundiales.
            </p>
          </div>
        </div>
        
        <div className="bg-white rounded-xl shadow-lg p-8 mb-12">
          <h2 className="text-3xl font-bold text-gray-800 mb-6 text-center">
            🎯 Características Principales
          </h2>
          
          <div className="grid grid-cols-1 md:grid-cols-2 gap-8">
            <div>
              <h3 className="text-xl font-semibold text-gray-800 mb-4">⚽ Gameplay</h3>
              <ul className="space-y-2 text-gray-600">
                <li>• Controles sin botones, solo gestos</li>
                <li>• Física del balón con curvas realistas</li>
                <li>• 8 trucos diferentes para dominar</li>
                <li>• IA inteligente con comportamientos únicos</li>
                <li>• Sistema de stamina y cansancio</li>
              </ul>
            </div>
            
            <div>
              <h3 className="text-xl font-semibold text-gray-800 mb-4">🎮 Modos de Juego</h3>
              <ul className="space-y-2 text-gray-600">
                <li>• Partidos rápidos</li>
                <li>• Modo Carrera</li>
                <li>• Torneos y Ligas</li>
                <li>• Futsal</li>
                <li>• Multijugador Online</li>
              </ul>
            </div>
            
            <div>
              <h3 className="text-xl font-semibold text-gray-800 mb-4">🎵 Audio & Visual</h3>
              <ul className="space-y-2 text-gray-600">
                <li>• Música libre de copyright</li>
                <li>• Efectos de sonido realistas</li>
                <li>• Gráficos optimizados para móviles</li>
                <li>• Animaciones fluidas</li>
                <li>• Efectos de partículas</li>
              </ul>
            </div>
            
            <div>
              <h3 className="text-xl font-semibold text-gray-800 mb-4">🌐 Online</h3>
              <ul className="space-y-2 text-gray-600">
                <li>• Ranking mundial en tiempo real</li>
                <li>• Estadísticas de jugadores</li>
                <li>• Competencias globales</li>
                <li>• Perfiles de usuario</li>
                <li>• Logros y recompensas</li>
              </ul>
            </div>
          </div>
        </div>
        
        <div className="bg-gradient-to-r from-green-600 to-blue-600 rounded-xl shadow-lg p-8 text-white text-center">
          <h2 className="text-3xl font-bold mb-4">
            🚀 ¡Próximamente!
          </h2>
          <p className="text-xl mb-6">
            Un juego de fútbol móvil que revolucionará la forma de jugar en dispositivos móviles
          </p>
          <Link
            to="/controles"
            className="bg-white text-green-600 px-8 py-4 rounded-full font-semibold text-lg hover:bg-gray-100 transition-all duration-300 shadow-lg hover:shadow-xl transform hover:-translate-y-1 inline-block"
          >
            🎮 Probar Controles Ahora
          </Link>
        </div>
      </div>
    </div>
  );
};

function App() {
  return (
    <div className="App">
      <BrowserRouter>
        <Routes>
          <Route path="/" element={<Home />} />
          <Route path="/controles" element={<TouchControlsDemo />} />
        </Routes>
      </BrowserRouter>
    </div>
  );
}

export default App;
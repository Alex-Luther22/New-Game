import React, { useState, useRef, useEffect } from 'react';

const TouchControlsDemo = () => {
  const canvasRef = useRef(null);
  const [isDrawing, setIsDrawing] = useState(false);
  const [currentGesture, setCurrentGesture] = useState([]);
  const [detectedTrick, setDetectedTrick] = useState('');
  const [ballPosition, setBallPosition] = useState({ x: 300, y: 300 });
  const [gestureTrail, setGestureTrail] = useState([]);
  const [actionType, setActionType] = useState('');
  const [kickPower, setKickPower] = useState(0);
  const [isCharging, setIsCharging] = useState(false);
  const [chargeStartTime, setChargeStartTime] = useState(0);
  const [touchStartPos, setTouchStartPos] = useState({ x: 0, y: 0 });
  const [touchEndPos, setTouchEndPos] = useState({ x: 0, y: 0 });
  const [showTrajectory, setShowTrajectory] = useState(false);
  const [trajectoryPoints, setTrajectoryPoints] = useState([]);

  // Configuración de trucos
  const trickPatterns = {
    'Roulette': 'Dibuja un círculo completo',
    'Elastico': 'Dibuja una L (derecha, luego abajo)',
    'Step-over': 'Dibuja un zigzag',
    'Nutmeg': 'Dibuja una línea vertical rápida',
    'Rainbow Flick': 'Dibuja un arco hacia arriba',
    'Rabona': 'Dibuja una curva externa',
    'Heel Flick': 'Dibuja hacia atrás',
    'Scorpion': 'Dibuja una S compleja'
  };

  const controlInstructions = {
    'Tap Simple': 'Toca una vez = Pase corto',
    'Doble Tap': 'Toca dos veces rápido = Disparo rápido',
    'Swipe Rápido': 'Desliza rápido = Disparo potente',
    'Swipe Lento': 'Desliza lento = Pase largo',
    'Mantener': 'Mantén presionado = Cargar patada',
    'Arrastrar': 'Arrastra = Mover jugador'
  };

  useEffect(() => {
    const canvas = canvasRef.current;
    const ctx = canvas.getContext('2d');
    
    // Configurar canvas
    canvas.width = 600;
    canvas.height = 400;
    
    // Limpiar canvas
    ctx.clearRect(0, 0, canvas.width, canvas.height);
    
    // Dibujar campo de fútbol
    drawField(ctx);
    
    // Dibujar balón
    drawBall(ctx, ballPosition.x, ballPosition.y);
    
    // Dibujar trail del gesto
    if (gestureTrail.length > 1) {
      drawGestureTrail(ctx, gestureTrail);
    }
    
    // Dibujar trayectoria predicha
    if (showTrajectory && trajectoryPoints.length > 1) {
      drawTrajectory(ctx, trajectoryPoints);
    }
    
    // Dibujar indicadores
    drawIndicators(ctx);
    
  }, [ballPosition, gestureTrail, trajectoryPoints, showTrajectory, kickPower, isCharging]);

  const drawField = (ctx) => {
    // Fondo verde
    ctx.fillStyle = '#4CAF50';
    ctx.fillRect(0, 0, 600, 400);
    
    // Líneas del campo
    ctx.strokeStyle = '#FFFFFF';
    ctx.lineWidth = 2;
    
    // Línea central
    ctx.beginPath();
    ctx.moveTo(300, 0);
    ctx.lineTo(300, 400);
    ctx.stroke();
    
    // Círculo central
    ctx.beginPath();
    ctx.arc(300, 200, 50, 0, 2 * Math.PI);
    ctx.stroke();
    
    // Áreas
    ctx.strokeRect(50, 100, 100, 200); // Área izquierda
    ctx.strokeRect(450, 100, 100, 200); // Área derecha
    
    // Porterías
    ctx.strokeRect(0, 150, 50, 100); // Portería izquierda
    ctx.strokeRect(550, 150, 50, 100); // Portería derecha
    
    // Punto de penalti
    ctx.fillStyle = '#FFFFFF';
    ctx.beginPath();
    ctx.arc(120, 200, 3, 0, 2 * Math.PI);
    ctx.fill();
    
    ctx.beginPath();
    ctx.arc(480, 200, 3, 0, 2 * Math.PI);
    ctx.fill();
  };

  const drawBall = (ctx, x, y) => {
    // Sombra del balón
    ctx.fillStyle = 'rgba(0, 0, 0, 0.3)';
    ctx.beginPath();
    ctx.ellipse(x + 2, y + 2, 12, 8, 0, 0, 2 * Math.PI);
    ctx.fill();
    
    // Balón
    ctx.fillStyle = '#FFFFFF';
    ctx.beginPath();
    ctx.arc(x, y, 10, 0, 2 * Math.PI);
    ctx.fill();
    
    // Líneas del balón
    ctx.strokeStyle = '#000000';
    ctx.lineWidth = 1;
    ctx.beginPath();
    ctx.moveTo(x - 7, y - 7);
    ctx.lineTo(x + 7, y + 7);
    ctx.moveTo(x - 7, y + 7);
    ctx.lineTo(x + 7, y - 7);
    ctx.stroke();
  };

  const drawGestureTrail = (ctx, trail) => {
    if (trail.length < 2) return;
    
    ctx.strokeStyle = '#FFD700';
    ctx.lineWidth = 3;
    ctx.lineCap = 'round';
    ctx.lineJoin = 'round';
    
    ctx.beginPath();
    ctx.moveTo(trail[0].x, trail[0].y);
    
    for (let i = 1; i < trail.length; i++) {
      const alpha = i / trail.length;
      ctx.globalAlpha = alpha;
      ctx.lineTo(trail[i].x, trail[i].y);
    }
    
    ctx.stroke();
    ctx.globalAlpha = 1.0;
  };

  const drawTrajectory = (ctx, points) => {
    if (points.length < 2) return;
    
    ctx.strokeStyle = '#FF4444';
    ctx.lineWidth = 2;
    ctx.setLineDash([5, 5]);
    
    ctx.beginPath();
    ctx.moveTo(points[0].x, points[0].y);
    
    for (let i = 1; i < points.length; i++) {
      ctx.lineTo(points[i].x, points[i].y);
    }
    
    ctx.stroke();
    ctx.setLineDash([]);
  };

  const drawIndicators = (ctx) => {
    // Indicador de carga de patada
    if (isCharging) {
      const chargeLevel = Math.min(kickPower / 100, 1);
      
      // Barra de carga
      ctx.fillStyle = 'rgba(0, 0, 0, 0.7)';
      ctx.fillRect(ballPosition.x - 25, ballPosition.y - 40, 50, 8);
      
      // Nivel de carga
      ctx.fillStyle = `hsl(${120 * (1 - chargeLevel)}, 100%, 50%)`;
      ctx.fillRect(ballPosition.x - 25, ballPosition.y - 40, 50 * chargeLevel, 8);
      
      // Círculo de potencia
      ctx.strokeStyle = `hsl(${120 * (1 - chargeLevel)}, 100%, 50%)`;
      ctx.lineWidth = 3;
      ctx.beginPath();
      ctx.arc(ballPosition.x, ballPosition.y, 20 + chargeLevel * 10, 0, 2 * Math.PI);
      ctx.stroke();
    }
    
    // Indicador de dirección
    if (touchStartPos.x && touchEndPos.x) {
      ctx.strokeStyle = '#FFD700';
      ctx.lineWidth = 3;
      ctx.beginPath();
      ctx.moveTo(touchStartPos.x, touchStartPos.y);
      ctx.lineTo(touchEndPos.x, touchEndPos.y);
      ctx.stroke();
      
      // Flecha
      const angle = Math.atan2(touchEndPos.y - touchStartPos.y, touchEndPos.x - touchStartPos.x);
      const arrowLength = 15;
      ctx.beginPath();
      ctx.moveTo(touchEndPos.x, touchEndPos.y);
      ctx.lineTo(
        touchEndPos.x - arrowLength * Math.cos(angle - Math.PI / 6),
        touchEndPos.y - arrowLength * Math.sin(angle - Math.PI / 6)
      );
      ctx.moveTo(touchEndPos.x, touchEndPos.y);
      ctx.lineTo(
        touchEndPos.x - arrowLength * Math.cos(angle + Math.PI / 6),
        touchEndPos.y - arrowLength * Math.sin(angle + Math.PI / 6)
      );
      ctx.stroke();
    }
  };

  const handleTouchStart = (e) => {
    const rect = canvasRef.current.getBoundingClientRect();
    const x = e.clientX - rect.left;
    const y = e.clientY - rect.top;
    
    setIsDrawing(true);
    setCurrentGesture([{ x, y, time: Date.now() }]);
    setGestureTrail([{ x, y }]);
    setTouchStartPos({ x, y });
    setTouchEndPos({ x, y });
    setIsCharging(true);
    setChargeStartTime(Date.now());
    setKickPower(0);
    setDetectedTrick('');
    setActionType('');
  };

  const handleTouchMove = (e) => {
    if (!isDrawing) return;
    
    const rect = canvasRef.current.getBoundingClientRect();
    const x = e.clientX - rect.left;
    const y = e.clientY - rect.top;
    
    setCurrentGesture(prev => [...prev, { x, y, time: Date.now() }]);
    setGestureTrail(prev => [...prev, { x, y }]);
    setTouchEndPos({ x, y });
    
    // Actualizar potencia de carga
    if (isCharging) {
      const chargeTime = Date.now() - chargeStartTime;
      const power = Math.min((chargeTime / 1000) * 100, 100);
      setKickPower(power);
    }
    
    // Calcular trayectoria predicha
    const direction = {
      x: x - touchStartPos.x,
      y: y - touchStartPos.y
    };
    
    if (Math.abs(direction.x) > 5 || Math.abs(direction.y) > 5) {
      const trajectory = calculateTrajectory(ballPosition, direction, kickPower);
      setTrajectoryPoints(trajectory);
      setShowTrajectory(true);
    }
  };

  const handleTouchEnd = () => {
    if (!isDrawing) return;
    
    setIsDrawing(false);
    setIsCharging(false);
    
    // Analizar el gesto
    const gestureResult = analyzeGesture(currentGesture);
    setDetectedTrick(gestureResult.trick);
    setActionType(gestureResult.action);
    
    // Ejecutar acción
    executeAction(gestureResult, touchStartPos, touchEndPos);
    
    // Limpiar después de 2 segundos
    setTimeout(() => {
      setGestureTrail([]);
      setShowTrajectory(false);
      setTouchStartPos({ x: 0, y: 0 });
      setTouchEndPos({ x: 0, y: 0 });
    }, 2000);
  };

  const analyzeGesture = (gesture) => {
    if (gesture.length < 2) {
      return { trick: 'Tap Simple', action: 'Pase corto' };
    }
    
    const distance = calculateDistance(gesture[0], gesture[gesture.length - 1]);
    const duration = gesture[gesture.length - 1].time - gesture[0].time;
    
    // Detectar doble tap
    if (distance < 20 && duration < 200) {
      return { trick: 'Doble Tap', action: 'Disparo rápido' };
    }
    
    // Detectar swipe
    if (distance > 50) {
      if (duration < 300) {
        return { trick: 'Swipe Rápido', action: 'Disparo potente' };
      } else {
        return { trick: 'Swipe Lento', action: 'Pase largo' };
      }
    }
    
    // Detectar trucos específicos
    const trickDetected = detectTrick(gesture);
    if (trickDetected) {
      return { trick: trickDetected, action: 'Truco' };
    }
    
    // Detectar carga
    if (duration > 500) {
      return { trick: 'Carga de Patada', action: 'Disparo cargado' };
    }
    
    return { trick: 'Movimiento', action: 'Mover jugador' };
  };

  const detectTrick = (gesture) => {
    if (gesture.length < 8) return null;
    
    // Detectar círculo (Roulette)
    if (isCircularGesture(gesture)) {
      return 'Roulette';
    }
    
    // Detectar L (Elastico)
    if (isLShapeGesture(gesture)) {
      return 'Elastico';
    }
    
    // Detectar zigzag (Step-over)
    if (isZigzagGesture(gesture)) {
      return 'Step-over';
    }
    
    // Detectar línea vertical (Nutmeg)
    if (isVerticalLineGesture(gesture)) {
      return 'Nutmeg';
    }
    
    // Detectar arco (Rainbow Flick)
    if (isArcGesture(gesture)) {
      return 'Rainbow Flick';
    }
    
    return null;
  };

  const isCircularGesture = (gesture) => {
    if (gesture.length < 8) return false;
    
    const center = calculateCenter(gesture);
    const radius = calculateAverageRadius(gesture, center);
    
    let circularPoints = 0;
    for (let point of gesture) {
      const distance = calculateDistance(point, center);
      if (Math.abs(distance - radius) < radius * 0.3) {
        circularPoints++;
      }
    }
    
    return circularPoints / gesture.length > 0.6;
  };

  const isLShapeGesture = (gesture) => {
    if (gesture.length < 6) return false;
    
    const midPoint = Math.floor(gesture.length / 2);
    const firstHalf = gesture.slice(0, midPoint);
    const secondHalf = gesture.slice(midPoint);
    
    return isHorizontalGesture(firstHalf) && isVerticalGesture(secondHalf);
  };

  const isZigzagGesture = (gesture) => {
    if (gesture.length < 6) return false;
    
    let directionChanges = 0;
    let lastDirection = { x: 0, y: 0 };
    
    for (let i = 1; i < gesture.length; i++) {
      const currentDirection = {
        x: gesture[i].x - gesture[i-1].x,
        y: gesture[i].y - gesture[i-1].y
      };
      
      if (lastDirection.x !== 0 || lastDirection.y !== 0) {
        const angle = Math.atan2(currentDirection.y, currentDirection.x) - 
                     Math.atan2(lastDirection.y, lastDirection.x);
        
        if (Math.abs(angle) > Math.PI / 4) {
          directionChanges++;
        }
      }
      
      lastDirection = currentDirection;
    }
    
    return directionChanges >= 2;
  };

  const isVerticalLineGesture = (gesture) => {
    if (gesture.length < 3) return false;
    
    const start = gesture[0];
    const end = gesture[gesture.length - 1];
    
    const horizontalDistance = Math.abs(end.x - start.x);
    const verticalDistance = Math.abs(end.y - start.y);
    
    return verticalDistance > horizontalDistance * 2;
  };

  const isArcGesture = (gesture) => {
    if (gesture.length < 5) return false;
    
    const start = gesture[0];
    const end = gesture[gesture.length - 1];
    const middle = gesture[Math.floor(gesture.length / 2)];
    
    const midLine = {
      x: (start.x + end.x) / 2,
      y: (start.y + end.y) / 2
    };
    
    return middle.y < midLine.y - 10; // Punto medio está por encima
  };

  const isHorizontalGesture = (gesture) => {
    if (gesture.length < 2) return false;
    
    const start = gesture[0];
    const end = gesture[gesture.length - 1];
    
    const horizontalDistance = Math.abs(end.x - start.x);
    const verticalDistance = Math.abs(end.y - start.y);
    
    return horizontalDistance > verticalDistance * 1.5;
  };

  const isVerticalGesture = (gesture) => {
    if (gesture.length < 2) return false;
    
    const start = gesture[0];
    const end = gesture[gesture.length - 1];
    
    const horizontalDistance = Math.abs(end.x - start.x);
    const verticalDistance = Math.abs(end.y - start.y);
    
    return verticalDistance > horizontalDistance * 1.5;
  };

  const calculateDistance = (point1, point2) => {
    const dx = point1.x - point2.x;
    const dy = point1.y - point2.y;
    return Math.sqrt(dx * dx + dy * dy);
  };

  const calculateCenter = (points) => {
    const sum = points.reduce((acc, point) => ({
      x: acc.x + point.x,
      y: acc.y + point.y
    }), { x: 0, y: 0 });
    
    return {
      x: sum.x / points.length,
      y: sum.y / points.length
    };
  };

  const calculateAverageRadius = (points, center) => {
    const totalRadius = points.reduce((acc, point) => 
      acc + calculateDistance(point, center), 0);
    return totalRadius / points.length;
  };

  const calculateTrajectory = (ballPos, direction, power) => {
    const points = [];
    const normalizedDirection = {
      x: direction.x / Math.sqrt(direction.x * direction.x + direction.y * direction.y),
      y: direction.y / Math.sqrt(direction.x * direction.x + direction.y * direction.y)
    };
    
    const velocity = {
      x: normalizedDirection.x * power * 0.1,
      y: normalizedDirection.y * power * 0.1
    };
    
    let currentPos = { x: ballPos.x, y: ballPos.y };
    
    for (let i = 0; i < 20; i++) {
      points.push({ x: currentPos.x, y: currentPos.y });
      
      currentPos.x += velocity.x;
      currentPos.y += velocity.y;
      
      // Aplicar resistencia
      velocity.x *= 0.95;
      velocity.y *= 0.95;
      
      // Verificar límites
      if (currentPos.x < 0 || currentPos.x > 600 || 
          currentPos.y < 0 || currentPos.y > 400) {
        break;
      }
    }
    
    return points;
  };

  const executeAction = (result, startPos, endPos) => {
    const direction = {
      x: endPos.x - startPos.x,
      y: endPos.y - startPos.y
    };
    
    const distance = Math.sqrt(direction.x * direction.x + direction.y * direction.y);
    
    if (distance > 10) {
      // Mover el balón
      const newBallPos = {
        x: Math.max(20, Math.min(580, ballPosition.x + direction.x * 0.3)),
        y: Math.max(20, Math.min(380, ballPosition.y + direction.y * 0.3))
      };
      
      // Animación del movimiento
      const steps = 10;
      let currentStep = 0;
      
      const animate = () => {
        if (currentStep < steps) {
          const progress = currentStep / steps;
          setBallPosition({
            x: ballPosition.x + (newBallPos.x - ballPosition.x) * progress,
            y: ballPosition.y + (newBallPos.y - ballPosition.y) * progress
          });
          currentStep++;
          setTimeout(animate, 50);
        } else {
          setBallPosition(newBallPos);
        }
      };
      
      animate();
    }
  };

  const resetDemo = () => {
    setBallPosition({ x: 300, y: 300 });
    setGestureTrail([]);
    setDetectedTrick('');
    setActionType('');
    setShowTrajectory(false);
    setTrajectoryPoints([]);
    setTouchStartPos({ x: 0, y: 0 });
    setTouchEndPos({ x: 0, y: 0 });
    setKickPower(0);
    setIsCharging(false);
  };

  return (
    <div className="max-w-6xl mx-auto p-6 bg-gradient-to-br from-green-50 to-blue-50 min-h-screen">
      <div className="bg-white rounded-xl shadow-2xl p-8">
        <h1 className="text-4xl font-bold text-center mb-8 text-gray-800">
          ⚽ Football Master - Controles Táctiles
        </h1>
        
        <div className="grid grid-cols-1 lg:grid-cols-3 gap-8">
          {/* Campo de Juego */}
          <div className="lg:col-span-2">
            <div className="bg-gradient-to-br from-green-600 to-green-700 p-4 rounded-lg">
              <h2 className="text-xl font-semibold text-white mb-4 text-center">
                🎮 Área de Pruebas Interactiva
              </h2>
              
              <div className="bg-white p-4 rounded-lg">
                <canvas
                  ref={canvasRef}
                  onMouseDown={handleTouchStart}
                  onMouseMove={handleTouchMove}
                  onMouseUp={handleTouchEnd}
                  onTouchStart={handleTouchStart}
                  onTouchMove={handleTouchMove}
                  onTouchEnd={handleTouchEnd}
                  className="border-2 border-green-800 rounded-lg cursor-crosshair w-full"
                  style={{ maxWidth: '600px', height: '400px' }}
                />
              </div>
              
              <div className="mt-4 text-center">
                <button
                  onClick={resetDemo}
                  className="bg-blue-500 hover:bg-blue-600 text-white px-6 py-2 rounded-lg font-semibold transition-colors"
                >
                  🔄 Reiniciar Demo
                </button>
              </div>
            </div>
            
            {/* Información en tiempo real */}
            <div className="mt-6 grid grid-cols-1 md:grid-cols-2 gap-4">
              <div className="bg-blue-100 p-4 rounded-lg">
                <h3 className="font-semibold text-blue-800 mb-2">🎯 Acción Detectada</h3>
                <p className="text-blue-700">
                  <strong>Gesto:</strong> {detectedTrick || 'Ninguno'}
                </p>
                <p className="text-blue-700">
                  <strong>Acción:</strong> {actionType || 'Ninguna'}
                </p>
              </div>
              
              <div className="bg-green-100 p-4 rounded-lg">
                <h3 className="font-semibold text-green-800 mb-2">⚡ Estado del Balón</h3>
                <p className="text-green-700">
                  <strong>Posición:</strong> ({Math.round(ballPosition.x)}, {Math.round(ballPosition.y)})
                </p>
                <p className="text-green-700">
                  <strong>Potencia:</strong> {Math.round(kickPower)}%
                </p>
              </div>
            </div>
          </div>
          
          {/* Panel de Instrucciones */}
          <div className="space-y-6">
            {/* Controles Básicos */}
            <div className="bg-gradient-to-br from-blue-600 to-blue-700 p-6 rounded-lg text-white">
              <h3 className="text-xl font-semibold mb-4">🎮 Controles Básicos</h3>
              <div className="space-y-3">
                {Object.entries(controlInstructions).map(([control, description]) => (
                  <div key={control} className="bg-white/20 p-3 rounded-lg">
                    <div className="font-semibold text-blue-100">{control}</div>
                    <div className="text-sm text-blue-50">{description}</div>
                  </div>
                ))}
              </div>
            </div>
            
            {/* Trucos Disponibles */}
            <div className="bg-gradient-to-br from-purple-600 to-purple-700 p-6 rounded-lg text-white">
              <h3 className="text-xl font-semibold mb-4">🌟 Trucos Disponibles</h3>
              <div className="space-y-3 max-h-64 overflow-y-auto">
                {Object.entries(trickPatterns).map(([trick, pattern]) => (
                  <div key={trick} className="bg-white/20 p-3 rounded-lg">
                    <div className="font-semibold text-purple-100">{trick}</div>
                    <div className="text-sm text-purple-50">{pattern}</div>
                  </div>
                ))}
              </div>
            </div>
            
            {/* Cómo Usar */}
            <div className="bg-gradient-to-br from-orange-600 to-orange-700 p-6 rounded-lg text-white">
              <h3 className="text-xl font-semibold mb-4">❓ Cómo Usar</h3>
              <div className="space-y-3 text-sm">
                <div className="bg-white/20 p-3 rounded-lg">
                  <strong>1.</strong> Usa el mouse o touch para dibujar gestos en el campo
                </div>
                <div className="bg-white/20 p-3 rounded-lg">
                  <strong>2.</strong> Observa cómo el sistema detecta tus movimientos
                </div>
                <div className="bg-white/20 p-3 rounded-lg">
                  <strong>3.</strong> Practica los trucos hasta dominarlos
                </div>
                <div className="bg-white/20 p-3 rounded-lg">
                  <strong>4.</strong> La línea dorada muestra tu gesto
                </div>
                <div className="bg-white/20 p-3 rounded-lg">
                  <strong>5.</strong> La línea roja punteada muestra la trayectoria
                </div>
              </div>
            </div>
          </div>
        </div>
        
        {/* Estadísticas y Características */}
        <div className="mt-8 grid grid-cols-1 md:grid-cols-3 gap-6">
          <div className="bg-gradient-to-br from-green-500 to-green-600 p-6 rounded-lg text-white text-center">
            <div className="text-3xl font-bold">⚽</div>
            <div className="text-2xl font-semibold">Física Realista</div>
            <div className="text-sm mt-2">Curvas, efectos Magnus, rebotes auténticos</div>
          </div>
          
          <div className="bg-gradient-to-br from-blue-500 to-blue-600 p-6 rounded-lg text-white text-center">
            <div className="text-3xl font-bold">🎮</div>
            <div className="text-2xl font-semibold">8 Trucos</div>
            <div className="text-sm mt-2">Roulette, Elastico, Step-over y más</div>
          </div>
          
          <div className="bg-gradient-to-br from-purple-500 to-purple-600 p-6 rounded-lg text-white text-center">
            <div className="text-3xl font-bold">📱</div>
            <div className="text-2xl font-semibold">Optimizado</div>
            <div className="text-sm mt-2">Funciona en Tecno Spark 8C y superiores</div>
          </div>
        </div>
        
        {/* Nota Importante */}
        <div className="mt-8 bg-yellow-50 border-l-4 border-yellow-400 p-6 rounded-lg">
          <div className="flex items-center">
            <div className="text-yellow-400 text-2xl mr-4">💡</div>
            <div>
              <h3 className="text-lg font-semibold text-yellow-800">¡Prueba Interactiva!</h3>
              <p className="text-yellow-700 mt-2">
                Esta es una demostración funcional de cómo funcionarán los controles en el juego de Unity. 
                Puedes probar todos los gestos y ver cómo el sistema los detecta en tiempo real. 
                ¡Perfecto para practicar antes de jugar!
              </p>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
};

export default TouchControlsDemo;
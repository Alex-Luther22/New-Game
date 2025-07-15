# 🎮 GUÍA COMPLETA: CONFIGURACIÓN UNITY PASO A PASO

## 📋 **PARTE 1: CONFIGURACIÓN INICIAL (10 MINUTOS)**

### **🎯 Paso 1: Crear Proyecto Unity**
1. **Abrir Unity Hub**
2. **Clic en "New Project"**
3. **Seleccionar "3D Core"**
4. **Nombre del proyecto: "FootballMaster"**
5. **Clic en "Create"**

### **🏷️ Paso 2: Configurar Layers (MUY IMPORTANTE)**
1. **Ir a Edit → Project Settings**
2. **Seleccionar "Tags and Layers"**
3. **En la sección "Layers" agregar:**
   ```
   Layer 8: Ground
   Layer 9: Player
   Layer 10: Ball
   Layer 11: HomeTeam
   Layer 12: AwayTeam
   Layer 13: UI
   Layer 14: Minimap
   Layer 15: Boundaries
   ```

### **🏷️ Paso 3: Configurar Tags**
1. **En la misma ventana, sección "Tags" agregar:**
   ```
   Ball
   Player
   Goal
   Ground
   Post
   Crossbar
   Grass
   Dirt
   Concrete
   HomePlayer
   AwayPlayer
   Goalkeeper
   Referee
   ```

### **📁 Paso 4: Crear Estructura de Carpetas**
1. **En la ventana Project, clic derecho en Assets**
2. **Create → Folder**
3. **Crear estas carpetas EXACTAMENTE:**
   ```
   Assets/
   ├── Scripts/
   │   ├── BallPhysics/
   │   ├── PlayerSystem/
   │   ├── TouchControls/
   │   ├── GameModes/
   │   ├── AI/
   │   ├── Audio/
   │   ├── UI/
   │   ├── Networking/
   │   └── Managers/
   ├── Prefabs/
   ├── Materials/
   ├── Audio/
   ├── Textures/
   ├── Models/
   ├── Animations/
   ├── Scenes/
   └── Resources/
   ```

---

## 📄 **PARTE 2: IMPORTAR SCRIPTS (5 MINUTOS)**

### **📂 Paso 5: Copiar Scripts a Unity**
1. **Navegar a la carpeta donde guardaste los scripts**
2. **Copiar cada archivo .cs a su carpeta correspondiente:**

   **📁 BallPhysics/ →**
   - `BallController.cs`

   **📁 PlayerSystem/ →**
   - `PlayerController.cs`
   - `PlayerData.cs`
   - `PlayerAI.cs`

   **📁 TouchControls/ →**
   - `TouchControlManager.cs`
   - `TrickDetector.cs`
   - `MobileControlsUI.cs`

3. **Esperar a que Unity compile** (verás una barra de progreso)

### **⚠️ Si hay errores de compilación:**
- **Ir a Console (Window → General → Console)**
- **Buscar errores en rojo**
- **Asegurarse de que TODOS los scripts estén en las carpetas correctas**

---

## 🎮 **PARTE 3: CREAR GAMEOBJECTS Y CONFIGURAR ESCENA (15 MINUTOS)**

### **🏟️ Paso 6: Crear Campo de Fútbol**
1. **Clic derecho en Hierarchy → 3D Object → Plane**
2. **Renombrar a "Field"**
3. **En Inspector:**
   - **Transform → Scale: X=10, Y=1, Z=15**
   - **Layer: Ground**
   - **Tag: Ground**
4. **Agregar componente NavMeshStaticFlags:**
   - **Clic en "Static" → marcar "Navigation Static"**

### **⚽ Paso 7: Crear Balón**
1. **Clic derecho en Hierarchy → 3D Object → Sphere**
2. **Renombrar a "Ball"**
3. **En Inspector:**
   - **Transform → Scale: X=0.22, Y=0.22, Z=0.22**
   - **Layer: Ball**
   - **Tag: Ball**
4. **Agregar componente Rigidbody:**
   - **Add Component → Physics → Rigidbody**
   - **Mass: 0.45**
   - **Drag: 0.05**
5. **Agregar script BallController:**
   - **Add Component → Scripts → BallController**

### **👥 Paso 8: Crear Jugadores**
1. **Clic derecho en Hierarchy → 3D Object → Capsule**
2. **Renombrar a "Player01"**
3. **En Inspector:**
   - **Transform → Scale: X=1, Y=1, Z=1**
   - **Layer: Player**
   - **Tag: Player**
4. **Agregar componente NavMeshAgent:**
   - **Add Component → AI → NavMeshAgent**
5. **Agregar scripts:**
   - **Add Component → Scripts → PlayerController**
   - **Add Component → Scripts → PlayerAI**
6. **Repetir para crear 21 jugadores más** (11 por equipo)

### **🥅 Paso 9: Crear Porterías**
1. **Clic derecho en Hierarchy → Create Empty**
2. **Renombrar a "Goal_Home"**
3. **En Inspector:**
   - **Transform → Position: X=0, Y=0, Z=-45**
   - **Tag: Goal**
4. **Agregar BoxCollider:**
   - **Add Component → Physics → BoxCollider**
   - **Is Trigger: ✓**
   - **Size: X=7, Y=2.5, Z=1**
5. **Repetir para "Goal_Away" en posición Z=45**

### **🎮 Paso 10: Configurar Controles**
1. **Clic derecho en Hierarchy → UI → Canvas**
2. **Renombrar a "MobileControlsCanvas"**
3. **En Inspector del Canvas:**
   - **Render Mode: Screen Space - Overlay**
   - **UI Scale Mode: Scale With Screen Size**
   - **Reference Resolution: 1920x1080**
4. **Agregar script MobileControlsUI:**
   - **Add Component → Scripts → MobileControlsUI**

### **📷 Paso 11: Configurar Cámara**
1. **Seleccionar Main Camera**
2. **En Inspector:**
   - **Transform → Position: X=0, Y=25, Z=-30**
   - **Transform → Rotation: X=45, Y=0, Z=0**

---

## 🎯 **PARTE 4: CONFIGURAR COMPONENTES ESPECÍFICOS (10 MINUTOS)**

### **🔧 Paso 12: Configurar BallController**
1. **Seleccionar el objeto "Ball"**
2. **En el script BallController, configurar:**
   - **Ball Mass: 0.43**
   - **Air Resistance: 0.04**
   - **Ground Friction: 0.3**
   - **Bounciness: 0.7**
   - **Magnus Effect: 1.2**
   - **Max Speed: 35**

### **🎮 Paso 13: Configurar MobileControlsUI**
1. **Seleccionar "MobileControlsCanvas"**
2. **En el script MobileControlsUI:**
   - **Joystick Range: 100**
   - **Button Size: 80**
   - **Joystick Size: 150**
   - **Trick Area Sensitivity: 1**

### **👥 Paso 14: Configurar PlayerController**
1. **Seleccionar cada jugador**
2. **En el script PlayerController, configurar:**
   - **Walk Speed: 3**
   - **Run Speed: 8**
   - **Sprint Speed: 12**
   - **Max Stamina: 100**
   - **Ball Control Radius: 1.2**

---

## 🎨 **PARTE 5: CREAR UI ELEMENTOS (10 MINUTOS)**

### **🕹️ Paso 15: Crear Joystick Virtual**
1. **Clic derecho en MobileControlsCanvas → UI → Image**
2. **Renombrar a "JoystickArea"**
3. **En Inspector:**
   - **Anchor: Bottom-Left**
   - **Pos X: 100, Pos Y: 100**
   - **Width: 150, Height: 150**
   - **Color: Blanco con Alpha 0.3**

4. **Clic derecho en JoystickArea → UI → Image**
5. **Renombrar a "JoystickBackground"**
6. **En Inspector:**
   - **Anchor: Center**
   - **Width: 150, Height: 150**
   - **Color: Blanco con Alpha 0.5**

7. **Clic derecho en JoystickBackground → UI → Image**
8. **Renombrar a "JoystickHandle"**
9. **En Inspector:**
   - **Anchor: Center**
   - **Width: 50, Height: 50**
   - **Color: Cyan**

### **🔘 Paso 16: Crear Botones de Acción**
1. **Clic derecho en MobileControlsCanvas → UI → Button**
2. **Renombrar a "PassButton"**
3. **En Inspector:**
   - **Anchor: Bottom-Right**
   - **Pos X: -100, Pos Y: 100**
   - **Width: 80, Height: 80**
4. **Cambiar texto a "PASE"**

5. **Repetir para crear:**
   - **ShootButton** (Pos Y: 200) - Texto: "DISPARO"
   - **SprintButton** (Pos X: -200, Pos Y: 150) - Texto: "SPRINT"
   - **TackleButton** (Pos X: -50, Pos Y: 150) - Texto: "TACKLE"

### **🌟 Paso 17: Crear Área de Trucos**
1. **Clic derecho en MobileControlsCanvas → UI → Image**
2. **Renombrar a "TrickArea"**
3. **En Inspector:**
   - **Anchor: Top-Center**
   - **Pos X: 0, Pos Y: -100**
   - **Width: 300, Height: 200**
   - **Color: Amarillo con Alpha 0.1**

4. **Clic derecho en TrickArea → UI → Text**
5. **Cambiar texto a "Área de Trucos - Dibuja gestos aquí"**

---

## 🔗 **PARTE 6: CONECTAR REFERENCIAS (5 MINUTOS)**

### **📎 Paso 18: Asignar Referencias en MobileControlsUI**
1. **Seleccionar MobileControlsCanvas**
2. **En el script MobileControlsUI, arrastrar:**
   - **JoystickArea → campo "Joystick Area"**
   - **JoystickBackground → campo "Joystick Background"**
   - **JoystickHandle → campo "Joystick Handle"**
   - **PassButton → campo "Pass Button"**
   - **ShootButton → campo "Shoot Button"**
   - **SprintButton → campo "Sprint Button"**
   - **TackleButton → campo "Tackle Button"**
   - **TrickArea → campo "Trick Area"**

### **📎 Paso 19: Asignar Referencias en PlayerController**
1. **Seleccionar cada jugador**
2. **En el script PlayerController, arrastrar:**
   - **Ball → campo "Ball Controller"**
   - **MobileControlsCanvas → campo "Mobile Controls UI"**

---

## 📱 **PARTE 7: CONFIGURAR PARA MÓVILES (5 MINUTOS)**

### **⚙️ Paso 20: Build Settings**
1. **File → Build Settings**
2. **Seleccionar "Android"**
3. **Clic en "Switch Platform"**
4. **Player Settings:**
   - **Company Name: Tu nombre**
   - **Product Name: Football Master**
   - **Default Orientation: Landscape Left**
   - **Minimum API Level: 24**
   - **Target API Level: 34**

### **🎮 Paso 21: Input Settings**
1. **Edit → Project Settings**
2. **XR Plug-in Management → Input System Package**
3. **Activar "Input System Package"**

### **🔧 Paso 22: Quality Settings**
1. **Edit → Project Settings → Quality**
2. **Crear perfil "Mobile Low":**
   - **Texture Quality: Half Res**
   - **Anti Aliasing: Disabled**
   - **Shadows: Hard Shadows Only**
   - **Shadow Distance: 50**

---

## 🎯 **PARTE 8: TESTING Y FINALIZACIÓN (10 MINUTOS)**

### **🧪 Paso 23: Crear NavMesh**
1. **Window → AI → Navigation**
2. **Seleccionar el objeto "Field"**
3. **En Navigation window, clic "Bake"**
4. **Esperar a que se complete el baking**

### **▶️ Paso 24: Probar el Juego**
1. **Clic en el botón "Play" ▶️**
2. **Verificar que:**
   - **Los controles aparezcan en pantalla**
   - **El joystick funcione**
   - **Los botones respondan**
   - **El área de trucos esté visible**

### **📱 Paso 25: Build para Android**
1. **File → Build Settings**
2. **Clic "Build"**
3. **Seleccionar carpeta para guardar APK**
4. **Esperar a que compile**

---

## 🚨 **SOLUCIÓN DE PROBLEMAS COMUNES**

### **❌ Error: "Script missing"**
**Solución:** Verificar que todos los scripts estén en las carpetas correctas

### **❌ Error: "NavMesh not found"**
**Solución:** 
1. Window → AI → Navigation
2. Seleccionar Field
3. Marcar "Navigation Static"
4. Clic "Bake"

### **❌ Error: "Touch not detected"**
**Solución:** Verificar que hay un EventSystem en la escena

### **❌ Error: "UI not visible"**
**Solución:** 
1. Verificar que Canvas esté en "Screen Space - Overlay"
2. Verificar que los objetos UI estén activos

### **❌ Error: "Players not moving"**
**Solución:**
1. Verificar que NavMeshAgent esté configurado
2. Verificar que el NavMesh esté bakeado
3. Verificar que los layers estén configurados correctamente

---

## ✅ **CHECKLIST FINAL**

- [ ] Proyecto Unity creado
- [ ] Layers configurados (8 layers)
- [ ] Tags configurados (13 tags)
- [ ] Estructura de carpetas creada
- [ ] Scripts copiados y compilados
- [ ] Campo de fútbol creado
- [ ] Balón configurado con BallController
- [ ] Jugadores creados con PlayerController y PlayerAI
- [ ] Porterías creadas
- [ ] UI Canvas con controles móviles
- [ ] Joystick virtual configurado
- [ ] Botones de acción creados
- [ ] Área de trucos configurada
- [ ] Referencias asignadas
- [ ] Build settings configurados
- [ ] NavMesh bakeado
- [ ] Juego probado en editor
- [ ] APK generado para Android

---

## 🎮 **¡LISTO PARA JUGAR!**

Tu juego de fútbol está completamente configurado y listo para usar. Los controles son:

- **🕹️ Joystick:** Mover jugador / Apuntar
- **🔘 PASE:** Pasar balón
- **🔘 DISPARO:** Disparar a portería
- **🔘 SPRINT:** Correr más rápido
- **🔘 TACKLE:** Entrada/Robar balón
- **🌟 Área de Trucos:** Dibujar gestos para trucos

**¡Disfruta tu juego de fútbol profesional!** ⚽🏆
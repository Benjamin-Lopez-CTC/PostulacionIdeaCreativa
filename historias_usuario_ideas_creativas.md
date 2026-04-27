# Historias de usuario ideas creativas

# Estructura básica de una historia de usuario
Como <TIPO_DE_USUARIO>
quiero <OBJETIVO_DESEADO>
para <RAZÓN_O_BENEFICIO>

## Registro de alumno
Como Alumno\
Quiero registrarme en el sitio\
Para despues crear un equipo en el que voy a estar con otro alumno

### Acceptance Criteria / Criterio de aceptación

- Como Alumno no registrado,
    - voy a la pagina de registro de alumno
    - ingreso nombreAlumno
    - ingreso apellidoAlumno
    - ingreso número de cédula
    - ingreso contraseña
    - confirmo contraseña
    - ENTONCES
        - el alumno no estaba registrado
        - veo el mensaje "registrado con exito"
        - me lleva a la pagina principal 

## Creación de equipos
Como equipoAlumnos
Quiero poder registrarme como Equipo con usuario y contraseña
Para poder acceder a la plataforma y postular mis ideas creativas.


### Acceptance Criteria / Criterio de aceptación

- Como equipo no registrado,
    - voy a la página de registro de equipo
    - ingreso nombreDeEquipo
    - ingreso password
    - ingreso comprobación de password
    - ingreso número de integrantes
    - selecciono los integrantes
    - ENTONCES
        - el nombre no estaba utilizado
        - los alumnos seleccionados estaban libres
        - veo el mensaje "Ingresado con exito"
        - me lleva a la pagina principal
    
    **
    - ENTONCES
        - el nombre si estaba utilizado
        - veo el mensaje "ya hay un equipo con ese nombre, ingrese otro nombre"
        - me muestra el form con los errores
    
    **
    - ENTONCES
        - el nombre no estaba utilizado
        - el password y la confirmacion no coinciden
        - veo el mensaje "los passwords no coinciden"
        - me muestra el form con los errores

    **
    - ENTONCES
        - el numero de integrantes es superior a 2
        - veo el mensaje "el numero de integrantes no esta permitido"
        - me muestra el form con los errores

    **
    - ENTONCES
        - el nombre no estaba utilizado
        - uno o mas alumnos ya estaban en un equipo
        - veo el mensaje "el/la alumno/a (nombreAlumno) ya esta en un equipo" o "los alumnos (nombreAlumno1) y (nombreAlumno2) ya estan en un equipo"
        - me muestra el form con los errores

## Postular idea
Como equipoAlumnos
Quiero postular una idea creativa
Para asegurarse que otro equipo no la haya postulado antes para el integrador.

### Acceptance Criteria / Criterios de aceptación
- Como usuario no registrado, 
    - voy a la pagina de postulacion de ideas
    - ingreso nombre del equipo
    - ingreso password del equipo
    - ingreso texto de la idea
    - hago click en el botón de postular idea
    - ENTONCES
    - veo un mensaje que dice:
        - "la idea fue postulada con éxito, espere por aprobación".
        - el sistema guarda la fecha y hora exacta de postulación

## Validar idea

Como profesor
Quiero revisar y aprobar las ideas postuladas
Para que los equipos sepan si sus ideas son validas y aprobadas.

### Acceptance Criteria / Criterio de Aceptación

- Como profesor
    - usando el user y pass hardcodeado
    - veo las ideas postuladas ordenadas por fecha y aprobadas o no.
    - veo que equipo postuló las ideas
    - hago click en una
    - ENTONCES
        - veo tres checkboxes que puedo modificar
        - es creativa
        - está bien planteada
        - aprobada
        - Marco todos los ticks
        - veo un mensaje "idea aprobada"
        - la idea correspondiente se marca como aprobada
        - me lleva a la pagina principal
    
    **
    - ENTONCES
        - es creativa
        - esta bien planteada
        - Ya se aprobo una idea similar de otro equipo
        - rechazada
        - marco los dos ticks correspondientes
        - veo un mensaje "idea rechazada"
        - la idea correspondiente se marca como rechazada
        - me lleva a la pagina principal

    **
    - ENTONCES
        - es creativa
        - no esta bien planteada
        - rechazada
        - marco el tick correspondiente
        - veo un mensaje "idea rechazada"
        - la idea correspondiente se marca como rechazada
        - me lleva a la pagina principal

    **
    - ENTONCES
        - no es creativa
        - rechazada
        - no marco ningun tick
        - veo un mensaje "idea rechazada"
        - la idea correspondiente se marca como rechazada
        - me lleva a la pagina principal
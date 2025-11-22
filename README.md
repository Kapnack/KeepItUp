 <img width="1024" height="364" alt="MainMenu" src="https://github.com/user-attachments/assets/645e0255-41fd-455b-a72d-eafb043fb1b7" />

“Keep it Up” es un juego con una mecánica sencilla pero desafiante, centrado en el equilibrio y la recolección de puntos. El jugador comienza seleccionando un nivel de dificultad, el cual afecta la agresividad del movimiento, la rotación y la escala del trampolín que mantiene al personaje en juego.
El objetivo principal es alcanzar 20 puntos antes de que transcurran 2 minutos, mientras se mantiene el equilibrio sobre el trampolín. Al conseguir los puntos, estos se envían automáticamente a la leaderboard de Android, fomentando la competencia basada en quién puede obtener la mayor cantidad de puntos dentro del tiempo límite, sin importar la dificultad seleccionada.
Al regresar al menú principal, si el jugador ha alcanzado los 20 puntos, puede acceder a la tienda para comprar y equipar diferentes skins, las cuales modifican la apariencia de la pelota del personaje y ofrecen una experiencia visual personalizada.
A su vez el juego incluye dos logros:

First Boot – desbloqueado al iniciar el juego por primera vez.

Fashionist – desbloqueado al comprar la primera skin en la tienda.

Este utiliza tres patrones de diseño utilizados en este trabajo práctico fueron:
* ServiceProvider / ServiceLocator: Su propósito es permitir la comunicación entre scripts sin necesidad de que todos estén directamente expuestos, evitando el uso excesivo de variables estáticas o patrones Singleton.
    De esta manera, los servicios pueden ser solicitados y obtenidos de forma centralizada y ordenada.

* Centralized Event System: Este patrón facilita la suscripción e invocación de eventos mediante una clase central que administra las notificaciones.
    Así, los objetos pueden comunicarse entre sí sin estar acoplados directamente.

* Object Pooling: Su objetivo es optimizar el uso de recursos evitando la creación y destrucción constante de objetos.
      En lugar de instanciar y destruir, se crean los objetos necesarios una sola vez, se desactivan cuando no se utilizan y se reutilizan cuando son requeridos nuevamente.
       Esto mejora el rendimiento y reduce la carga de procesamiento.

A su vez, este juego incluye un plugin programado en Java mediante Android Studio, el cual se encarga de almacenar los logs generados por Unity y mostrarlos en una consola dentro del propio juego.

* IMAGENES:

* ICONO:
<img width="512" height="512" alt="Icon" src="https://github.com/user-attachments/assets/26cb249a-73c7-4615-ac01-71477c4e0706" />

* CAPTURAS DE PANTALLA:
<img width="2562" height="1216" alt="ScreenShot_framed" src="https://github.com/user-attachments/assets/b52953e8-e7d7-4f56-af7b-4d0c6d559876" />
<img width="2562" height="1216" alt="ScreenShot2_framed" src="https://github.com/user-attachments/assets/1b6b9c41-a8d5-4fe7-9e01-6c58f601d602" />

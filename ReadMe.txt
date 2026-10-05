Щоб скачати оригінальний проект перейдіть за цим посиланням: https://github.com/eugeniy3339/2D-Movement-Eugen-Durbalo
To download original project go to this web: https://github.com/eugeniy3339/2D-Movement-Eugen-Durbalo

UA:

Цей проект я робив більше для себе, бо витрачати цілий день на програмування руху персонажа не дуже мені подобалося. Особливо коли я був на гем джемах, на розробку гри у котрих давалося 2-3 дні.
Цей проект повністю відкритий для кожного користувача інтернету, але всеж таки я хотів би вас попросити не сильно часто використовувати різні готові асети, бо це може погано сказатися на ваші навички в програмуванні.

Цей проект має повністю модульну систему руху персонажа.
Отож ви можете легко додавати нові модулі під свої потреби.

Ви можете знайти префаб гравця у теці Prefabs, 
або створити нового. Для цього створіть новий об'єкт, наприклад капсулу. До цього об'єкту додайте компоненти Rigidbody, та MovementManager (Обов'язковий компонент, без нього майже усі компоненти працювати не будуть).
Створіть дочерній об'єкт та задайте йому позицію там де у гравця знаходяться ноги (бажано там, де у нього закінчується колайдер) та оберіть цей об'єкт як feet pos у Movement Manager компоненті. 
Створіть шар для землі (або можете використовувати вже існуючий шар). Та оберіть його як шар землі у Movement Manager.
Тепер у вас на сцені є новий гравець. Ви можете додавати до нього різні модулі як Dash, Jump, та Run.

Щоб додати гравцеві інпути просто додайте йому компонент Player Inputs Manager.
Графіку гравця контролює компонент Character Visuals Handler.
У проекті вже є свій передстворенний Animator Controller, ви можете або замінити у ньому анімації, або створити Animator Override Controller (рекомендований спосіб) та замінити анімації вже в ньому.

Також ви можете спокійно переписувати скрипти під себе, та розповсюджувати цей проект (https://en.wikipedia.org/wiki/MIT_License).



EN:

This project was made mostly for myself, as I didn’t enjoy spending an entire day programming basic character movement—especially during game jams where you only have 2–3 days to make a full game.
The project is fully open to anyone on the internet, but I’d still like to ask you not to rely too heavily on pre-made assets, as that can negatively impact your programming skills in the long run.

This project features a fully modular character movement system,
so you can easily add new modules tailored to your needs.

You can find a player prefab in the Prefabs folder,
or you can create your own. To do this, create a new object (e.g., a capsule) and add the following components: Rigidbody, Player Movement Manager (the needed component, without it some components may not work).
Create new Empty child component and place it where player's feet are placed (better to be on the end of collider) and choose these object as feet pos in Movement Manager component.
Create a new Ground Layer (or use existing one) and chose it as a ground layer in Movement Manager.
Now you have a working player in your scene. You can add various modules to it, such as Dash, Jump, and Run.

To add inputs to player just add Player Inputs Manager component to it.
Player visuals are being controlled by Character Visuals Handler.
There is an existing Animator Controller in the project, you can change its animations, or create a new Animator Override Controller (recomended way) and change them there.

Feel free to modify the scripts and distribute this project as you wish — https://en.wikipedia.org/wiki/MIT_License



Credits: 
Eugen Durbalo - Developement
2D Pixel Art Character Template Asset Pack - https://zegley.itch.io/2d-platformermetroidvania-asset-pack
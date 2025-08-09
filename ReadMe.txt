Щоб скачати оригінальний проект перейдіть за цим посиланням: https://github.com/eugeniy3339/2D-Movement-Eugen-Durbalo
To download original project go to this web: https://github.com/eugeniy3339/2D-Movement-Eugen-Durbalo

UA:

Цей проект я робив більше для себе, бо витрачати цілий день на програмування руху персонажа не дуже мені подобалося. Особливо коли я був на гем джемах, на розробку гри у котрих давалося 2-3 дні.
Цей проект повністю відкритий для кожного користувача інтернету, але всеж таки я хотів би вас попросити не сильно часто використовувати різні готові асети, бо це може погано сказатися на ваші навички в програмуванні.

Цей проект має повністю модульну систему руху персонажа.
Отож ви можете легко додавати нові модулі під свої потреби.

Ви можете знайти префаб гравця у теці Prefabs, 
або створити нового. Для цього створіть новий об'єкт, наприклад капсулу. До цього об'єкту додайте компоненти Rigidbody, Player, PlayerInputsManager та скрипт руху (PlatformerMovement/TopDownMovement в залежності від вашої потреби), або оберіть тип мувементу в компоненті Player.
Тепер у вас на сцені є новий гравець. Ви можете додавати до нього різні модулі як Dash, Jump (не працює з Top Down рухом), та Run (Це поки-що усі модулі присутні в проекті).

Також ви можете налаштовувати усілякі змінні під свої потреби, наприклад швидкість, висота стрибку, максимальний кут по котрому може ходити гравець, висоа гравця (потрібно для визначення довжини рей касту)...

Також ви можете спокійно переписувати скрипти під себе, та розповсюджувати цей проект (https://en.wikipedia.org/wiki/MIT_License).

EN:

This project was made mostly for myself, as I didn’t enjoy spending an entire day programming basic character movement—especially during game jams where you only have 2–3 days to make a full game.
The project is fully open to anyone on the internet, but I’d still like to ask you not to rely too heavily on pre-made assets, as that can negatively impact your programming skills in the long run.

This project features a fully modular character movement system,
so you can easily add new modules tailored to your needs.

You can find a player prefab in the Prefabs folder,
or you can create your own. To do this, create a new object (e.g., a capsule) and add the following components: Rigidbody, Player, PlayerInputsManager, and the movement script (PlatformerMovement or TopDownMovement, depending on your needs).
Alternatively, you can simply choose the movement type in the Player component.
Now you have a working player in your scene. You can add various modules to it, such as Dash, Jump (not compatible with Top Down movement), and Run (currently, these are all the modules included in the project).

You can also customize various variables to suit your needs—for example, movement speed, jump height, the maximum slope angle the player can walk on, player height (used for raycast length), and more.

Feel free to modify the scripts and distribute this project as you wish — https://en.wikipedia.org/wiki/MIT_License

Credits: 
2D Pixel Art Character Template Asset Pack - https://zegley.itch.io/2d-platformermetroidvania-asset-pack

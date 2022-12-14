# Infinite Defense
![Game icon](https://github.com/Aywi-style/InfiniteDefense/raw/main/Media/icon.png)

*Айдлер по мотивам tower defense с элементами тайкуна.*  
[GooglePlay](https://play.google.com/store/apps/details?id=com.ZakiGames.InfinityDefence "Application page")

Моя работа заключалась в создании боевых систем, настройке игрового баланса, общем видении проекта, визуальном видении проекта и детальной проработке анимаций объектов.  
Впервые работал со ScriptableObject'ами и настройками Inspector'а.  

Мною реализованы механики:  
• Контекстное переключение инструментов главного героя: около руды достаётся кирка, во время ближнего боя - меч, во время дальнего боя - лук;  
• Создание помошников главного героя (в дальнейшем "защитников) с корректными данными внутри них;  
• Кастомная, следящая за гг камера;  
• Зоны обнаружения: главного героя, противников, защитных башен, защитников;  
• Разные состояния юнитов: бежит, стоит, в бою и так далее;  
• Реакция на попадание различных объектов в зоны обнаружения;  
• Динамичная отрисовка зоны обнаружения у защитных башен;  
• Прокачал систему урона, переведя её на ECS ивенты;  
• Регенерация здоровья главного героя;  
• Воскрешение защитников и главного героя;  
• Таргетирование на преоритетном объекте;  
• Отслеживание цели;
• Механика весов объектов. Чем выше вес, тем вероятнее вражеский субъект перключится на этот конкретный объект;  
• Стрельба пушек на защитных башнях: кручение вокруг своей оси, смещение пушки, непосредственно сама стрельба, перезарядка, эффекты стрельбы и попадания;  
• Работа снарядов взрывающихся и не взрывающихся: полёт по дуге используя псевдогравитацию, взрыв в точке попадания;  
• Система смертей;  
• Передвижение юнитов и нахождение оптимальных путей с помощью NavMesh;  
• Передвижение корабля с целым энкаунтером врагов;  
• Вибрация во время различных игровых действий, используя NiceVibrations фреймворк;

![Game poster](https://github.com/Aywi-style/InfiniteDefense/raw/main/Media/img_1.png)
![Gameplay_1](https://github.com/Aywi-style/InfiniteDefense/raw/main/Media/img_2.png)
![Gameplay_2](https://github.com/Aywi-style/InfiniteDefense/raw/main/Media/img_3.png)
![Gameplay_3](https://github.com/Aywi-style/InfiniteDefense/raw/main/Media/img_4.png)
![Gameplay_4](https://github.com/Aywi-style/InfiniteDefense/raw/main/Media/img_5.png)
![Gameplay_5](https://github.com/Aywi-style/InfiniteDefense/raw/main/Media/img_6.png)

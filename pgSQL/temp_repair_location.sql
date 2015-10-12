-- Function: temp_repair_location()

-- DROP FUNCTION temp_repair_location();

CREATE OR REPLACE FUNCTION temp_repair_location()
  RETURNS integer AS
$BODY$
BEGIN
/*
   -- Зарежда новите за обработка
   UPDATE n_template_location SET tl_name = post_location WHERE tl_name IS NULL;

   -- Област: Благоевград
   PERFORM temp_update_location_province(1, 'Област Благоевград|обл. Благоевград');
   -- Област: Бургас
   PERFORM temp_update_location_province(2, 'Област Бургас|обл. Бургас');
   -- Област: Варна
   PERFORM temp_update_location_province(3, 'Област Варна|обл. Варна');
   -- Област: Велико Търново
   PERFORM temp_update_location_province(4, 'Област Велико Търново|обл. Велико Търново');
   -- Област: Видин
   PERFORM temp_update_location_province(5, 'Област Видин|обл. Видин');
   -- Област: Враца
   PERFORM temp_update_location_province(6, 'Област Враца|обл. Враца');
   -- Област: Габрово
   PERFORM temp_update_location_province(7, 'Област Габрово|обл. Габрово');
   -- Област: Добрич
   PERFORM temp_update_location_province(8, 'Област Добрич|обл. Добрич');
   -- Област: Кърджали
   PERFORM temp_update_location_province(9, 'Област Кърджали|обл. Кърджали');
   -- Област: Кюстендил
   PERFORM temp_update_location_province(10, 'Област Кюстендил|обл. Кюстендил');
   -- Област: Ловеч
   PERFORM temp_update_location_province(11, 'Област Ловеч|обл. Ловеч');
   -- Област: Монтана
   PERFORM temp_update_location_province(12, 'Област Монтана|обл. Монтана');
   -- Област: Пазарджик
   PERFORM temp_update_location_province(13, 'Област Пазарджик|обл. Пазарджик');
   -- Област: Перник
   PERFORM temp_update_location_province(14, 'Област Перник|обл. Перник');
   -- Област: Плевен
   PERFORM temp_update_location_province(15, 'Област Плевен|обл. Плевен');
   -- Област: Пловдив
   PERFORM temp_update_location_province(16, 'Област Пловдив|обл. Пловдив');
   -- Област: Разград
   PERFORM temp_update_location_province(17, 'Област Разград|обл. Разград');
   -- Област: Русе
   PERFORM temp_update_location_province(18, 'Област Русе|, обл. Русе');
   -- Област: Силистра
   PERFORM temp_update_location_province(19, 'Област Силистра|обл. Силистра');
   -- Област: Сливен
   PERFORM temp_update_location_province(20, 'Област Сливен|обл. Сливен');
   -- Област: Смолян
   PERFORM temp_update_location_province(21, 'Област Смолян|обл. Смолян');
   -- Област: София-област
   PERFORM temp_update_location_province(22, 'Област София-област|обл. София област');
   -- Област: София-град
   PERFORM temp_update_location_province(23, 'Област София-град|обл. София-град');
   -- Област: Стара Загора
   PERFORM temp_update_location_province(24, 'Област Стара Загора|обл. Стара Загора');
   -- Област: Търговище
   PERFORM temp_update_location_province(25, 'Област Търговище|обл. Търговище');
   -- Област: Хасково
   PERFORM temp_update_location_province(26, 'Област Хасково|, обл. Хасково');
   -- Област: Шумен
   PERFORM temp_update_location_province(27, 'Област Шумен|обл. Шумен');
   -- Област: Ямбол
   PERFORM temp_update_location_province(28, 'Област Ямбол|обл. Ямбол');

   -- Благоевград
   PERFORM temp_update_location_town(1, 'гр. Благоевград', 1);
   -- Бургас
   PERFORM temp_update_location_town(2, 'гр. Бургас', 2);
   -- Варна
   PERFORM temp_update_location_town(3, 'гр. Варна', 3);
   -- Велико Търново
   PERFORM temp_update_location_town(4, 'гр. Велико Търново', 4);
   -- Видин
   PERFORM temp_update_location_town(5, 'гр. Видин', 5);
   -- Враца
   PERFORM temp_update_location_town(6, 'гр. Враца', 6);
   -- Габрово
   PERFORM temp_update_location_town(7, 'гр. Габрово', 7);
   -- Добрич
   PERFORM temp_update_location_town(8, 'гр. Добрич', 8);
   -- Кърджали
   PERFORM temp_update_location_town(9, 'гр. Кърджали', 9);
   -- Кюстендил
   PERFORM temp_update_location_town(10, 'гр. Кюстендил', 10);
   -- Ловеч
   PERFORM temp_update_location_town(11, 'гр. Ловеч', 11);
   -- Монтана
   PERFORM temp_update_location_town(12, 'гр. Монтана', 12);
   -- Пазарджик
   PERFORM temp_update_location_town(13, 'гр. Пазарджик', 13);
   -- Перник
   PERFORM temp_update_location_town(14, 'гр. Перник', 14);
   -- Плевен
   PERFORM temp_update_location_town(15, 'гр. Плевен', 15);
   -- Пловдив
   PERFORM temp_update_location_town(16, 'гр. Пловдив', 16);
   -- Разград
   PERFORM temp_update_location_town(17, 'гр. Разград', 17);
   -- Русе
   PERFORM temp_update_location_town(18, 'гр. Русе', 18);
   -- Силистра
   PERFORM temp_update_location_town(19, 'гр. Силистра', 19);
   -- Сливен
   PERFORM temp_update_location_town(20, 'гр. Сливен', 20);
   -- Смолян
   PERFORM temp_update_location_town(21, 'гр. Смолян', 21);
   -- София
   PERFORM temp_update_location_town(22, 'гр. София', 23);
   -- Стара Загора
   PERFORM temp_update_location_town(23, 'гр. Стара Загора', 24);
   -- Търговище
   PERFORM temp_update_location_town(24, 'гр. Търговище', 25);
   -- Хасково
   PERFORM temp_update_location_town(25, 'гр. Хасково', 26);
   -- Шумен
   PERFORM temp_update_location_town(26, 'гр. Шумен', 27);
   -- Ямбол
   PERFORM temp_update_location_town(27, 'гр. Ямбол', 28);
   -- Сандански
   PERFORM temp_update_location_town(28, 'гр. Сандански', 1);
   -- Айтос
   PERFORM temp_update_location_town(29, 'гр. Айтос', 2);
   -- Китен
   PERFORM temp_update_location_town(30, 'гр. Китен', 2);
   -- Роман
   PERFORM temp_update_location_town(31, 'гр. Роман', 6);
   -- Поморие
   PERFORM temp_update_location_town(32, 'гр. Поморие', 2);
   -- Бобошево
   PERFORM temp_update_location_town(33, 'гр. Бобошево', 10);
   -- Бойчиновци
   PERFORM temp_update_location_town(34, 'гр. Бойчиновци', 12);
   -- Дулово
   PERFORM temp_update_location_town(35, 'гр. Дулово', 19);





   -- Зарежда областа от разпознатия град
   UPDATE n_template_location
      SET t_province_id = t.t_province_id
   FROM t_town t 
   WHERE n_template_location.t_town_id IS NOT NULL
     AND n_template_location.t_province_id IS NULL
     AND t.id = n_template_location.t_town_id;
*/

   UPDATE n_template_location
      SET t_location_1 = trim(l.p_location[1])
	, t_location_2 = trim(l.p_location[2])
	, t_location_3 = trim(l.p_location[3])
   FROM (
	SELECT tl.id, regexp_split_to_array(tl.post_location, E',') AS p_location
	FROM n_template_location tl
	) l
   WHERE n_template_location.id = l.id;

     
  RETURN 0;
END;
$BODY$
  LANGUAGE plpgsql VOLATILE
  COST 100;
ALTER FUNCTION temp_repair_location()
  OWNER TO postgres;

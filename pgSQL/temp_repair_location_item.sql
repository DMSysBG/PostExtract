-- Function: temp_repair_location_item()

-- DROP FUNCTION temp_repair_location_item();

CREATE OR REPLACE FUNCTION temp_repair_location_item()
  RETURNS integer AS
$BODY$
BEGIN
-- SELECT temp_repair_location_item();
/*
   UPDATE n_template_location
      SET n_template_province_id = NULL
        , n_template_town_id = NULL
	, n_template_region_id = NULL;

   DELETE FROM n_template_province;
   ALTER SEQUENCE n_template_province_id_seq RESTART WITH 1;

   DELETE FROM n_template_town;
   ALTER SEQUENCE n_template_town_id_seq RESTART WITH 1;
   
   DELETE FROM n_template_region;
   ALTER SEQUENCE n_template_region_id_seq RESTART WITH 1;
*/

   -- Изчиства таблицата за обработка
   DELETE FROM n_template_location_item;

   -- Зарежда новите локации за обработка
   INSERT INTO n_template_location_item
   ( n_template_location_id, t_location_name )
   SELECT tl.id AS n_template_location_id
	, trim(regexp_split_to_table(tl.post_location, E',')) AS p_location
   FROM n_template_location tl
   WHERE tl.n_template_province_id IS NULL
      OR tl.n_template_town_id IS NULL
      OR tl.n_template_region_id IS NULL;

   -- Област
   UPDATE n_template_location_item
      SET t_location_type_id = 1
	, t_location_name = trim(regexp_replace(t_location_name, 'Област |обл\. ', '', 'g'))
   WHERE t_location_name ~* 'Област |обл\. '
     AND t_location_type_id IS NULL;
   -- Град
   UPDATE n_template_location_item
      SET t_location_type_id = 2
	, t_location_name = trim(regexp_replace(t_location_name, 'гр\. ', '', 'g'))
   WHERE t_location_name ~* 'гр\. '
     AND t_location_type_id IS NULL;
   -- Село
   UPDATE n_template_location_item
      SET t_location_type_id = 3
	, t_location_name = trim(regexp_replace(t_location_name, 'с\. ', '', 'g'))
   WHERE t_location_name ~* 'с\. '
     AND t_location_type_id IS NULL;
   -- Курортен комплекс
   UPDATE n_template_location_item
      SET t_location_type_id = 4
	, t_location_name = trim(regexp_replace(t_location_name, 'к\.к\. ', '', 'g'))
   WHERE t_location_name ~* 'к\.к\. '
     AND t_location_type_id IS NULL;
   -- Mестност
   UPDATE n_template_location_item
      SET t_location_type_id = 5
	, t_location_name = trim(regexp_replace(t_location_name, 'м-т ', '', 'g'))
   WHERE t_location_name ~* 'м-т '
     AND t_location_type_id IS NULL;
   -- Вилна зона
   UPDATE n_template_location_item
      SET t_location_type_id = 6
	, t_location_name = trim(regexp_replace(t_location_name, 'в\.з\. ', '', 'g'))
   WHERE t_location_name ~* 'в\.з\. '
     AND t_location_type_id IS NULL;
     
   -- Язовир
   UPDATE n_template_location_item
      SET t_location_type_id = 8
	, t_location_name = trim(regexp_replace(t_location_name, 'яз\. ', '', 'g'))
   WHERE t_location_name ~* 'яз\. '
     AND t_location_type_id IS NULL;

   -- Хижа
   UPDATE n_template_location_item
      SET t_location_type_id = 9
	, t_location_name = trim(regexp_replace(t_location_name, 'хижа ', '', 'g'))
   WHERE t_location_name ~* 'хижа '
     AND t_location_type_id IS NULL;
     
   -- Район
   UPDATE n_template_location_item
      SET t_location_type_id = 7
   FROM n_template_location_item tp	-- има област
      , n_template_location_item tt	-- има град
   WHERE n_template_location_item.t_location_type_id IS NULL
     -- има област
     AND tp.n_template_location_id = n_template_location_item.n_template_location_id
     AND tp.t_location_type_id = 1
     -- има град
     AND tt.n_template_location_id = n_template_location_item.n_template_location_id
     AND tt.t_location_type_id IN (2, 3, 4);

   -- Област
   -- Разпознава областите
   PERFORM temp_recognize_location_province();

   -- Добавя неразпознатие области
   INSERT INTO n_template_province 
   (tp_name, t_location_type_id)
   SELECT DISTINCT tli.t_location_name
        , tli.t_location_type_id
   FROM n_template_location tl
   INNER JOIN n_template_location_item tli
      ON tli.n_template_location_id = tl.id
     AND tli.t_location_type_id = 1
   WHERE tl.n_template_province_id IS NULL;

   -- Разпознава области / отново /
   PERFORM temp_recognize_location_province();


   -- Градове
   -- Разпознава градовете към области
   PERFORM temp_recognize_location_town();

   -- Добавя неразпознатие градове
   INSERT INTO n_template_town
   (n_template_province_id, tt_name, t_location_type_id)
   SELECT DISTINCT tl.n_template_province_id
	, tli.t_location_name
	, tli.t_location_type_id
   FROM n_template_location tl
   INNER JOIN n_template_location_item tli
      ON tli.n_template_location_id = tl.id
     AND tli.t_location_type_id IN (2, 3, 4, 8, 9)
   WHERE tl.n_template_town_id IS NULL
     AND tl.n_template_province_id IS NOT NULL;

   -- Разпознава градовете към области / отново /
   PERFORM temp_recognize_location_town();


   -- Райони
   -- Разпознава районите към градовете
   PERFORM temp_recognize_location_region();

   -- Разпознава райони като тип
   UPDATE n_template_location_item
      SET t_location_type_id = 7
   FROM n_template_location tl
   WHERE tl.n_template_province_id IS NOT NULL
     AND tl.n_template_town_id IS NOT NULL
     AND tl.n_template_region_id IS NULL
     AND n_template_location_item.n_template_location_id = tl.id
     AND n_template_location_item.t_location_type_id IS NULL;
     
   -- Добавя неразпознатие райони
   INSERT INTO n_template_region
   (n_template_town_id , tr_name, t_location_type_id)
   SELECT DISTINCT tl.n_template_town_id 
	, tli.t_location_name
	, tli.t_location_type_id
   FROM n_template_location tl
   INNER JOIN n_template_location_item tli
      ON tli.n_template_location_id = tl.id
     AND tli.t_location_type_id IN ( 6, 7)
   WHERE tl.n_template_region_id IS NULL;

   -- Добавя неразпознатие местности като райони
   INSERT INTO n_template_region
   (n_template_town_id , tr_name, t_location_type_id)
   SELECT DISTINCT tl.n_template_town_id 
	, tli.t_location_name
	, tli.t_location_type_id
   FROM n_template_location tl
   INNER JOIN n_template_location_item tli
      ON tli.n_template_location_id = tl.id
     AND tli.t_location_type_id = 5
   WHERE tl.n_template_region_id IS NULL
     AND tl.n_template_town_id IS NOT NULL
     AND tl.n_template_province_id IS NOT NULL;

   -- Разпознава районите към градовете / отново /
   PERFORM temp_recognize_location_region();


   -- Градове
   -- Добавя останалите местности като градове
   INSERT INTO n_template_town
   (n_template_province_id, tt_name, t_location_type_id)
   SELECT DISTINCT tl.n_template_province_id
	, tli.t_location_name
	, tli.t_location_type_id
   FROM n_template_location tl
   INNER JOIN n_template_location_item tli
      ON tli.n_template_location_id = tl.id
     AND tli.t_location_type_id = 5
   WHERE tl.n_template_region_id IS NULL
     AND tl.n_template_town_id IS NULL
     AND tl.n_template_province_id IS NOT NULL;

   -- Разпознава градовете към области / отново /
   PERFORM temp_recognize_location_town();


   -- ---
   -- Маркира останалите локации без райони
   UPDATE n_template_location
      SET n_template_region_id = 0
   FROM n_template_location_item tli
   WHERE n_template_location.n_template_province_id IS NOT NULL
     AND n_template_location.n_template_town_id IS NOT NULL
     AND n_template_location.n_template_region_id IS NULL
     AND tli.n_template_location_id = n_template_location.id
     AND tli.t_location_type_id IS NOT NULL;
     
  RETURN 0;
END;
$BODY$
  LANGUAGE plpgsql VOLATILE
  COST 100;
ALTER FUNCTION temp_repair_location_item()
  OWNER TO postgres;

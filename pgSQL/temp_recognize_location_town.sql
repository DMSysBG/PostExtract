-- Function: temp_recognize_location_town()

-- DROP FUNCTION temp_recognize_location_town();

CREATE OR REPLACE FUNCTION temp_recognize_location_town()
  RETURNS integer AS
$BODY$
BEGIN
   -- Разпознава градовете към области
   UPDATE n_template_location
      SET n_template_town_id = tt.id

   FROM n_template_location_item tli
      , n_template_town tt

   WHERE n_template_location.n_template_town_id IS NULL				-- Неразпознати градове
     AND tli.n_template_location_id = n_template_location.id
     AND tli.t_location_type_id IN (2, 3, 4, 5, 8, 9)
										-- Търси съответствия
     AND tt.n_template_province_id = n_template_location.n_template_province_id
     AND tt.t_location_type_id = tli.t_location_type_id
     AND lower(tt.tt_name) = lower(tli.t_location_name);


   -- Разпознава градовете без области
   UPDATE n_template_location
      SET n_template_town_id = tt.id
        , n_template_province_id = tt.n_template_province_id

   FROM n_template_location_item tli
      , n_template_town tt

   WHERE n_template_location.n_template_town_id IS NULL				-- Неразпознати градове
     AND n_template_location.n_template_province_id IS NULL			-- Неразпознати области
     AND tli.n_template_location_id = n_template_location.id
     AND tli.t_location_type_id = 2
										-- Търси съответствия
     AND tt.t_location_type_id = tli.t_location_type_id
     AND lower(tt.tt_name) = lower(tli.t_location_name);
     
  RETURN 0;
END;
$BODY$
  LANGUAGE plpgsql VOLATILE
  COST 100;
ALTER FUNCTION temp_recognize_location_town()
  OWNER TO postgres;
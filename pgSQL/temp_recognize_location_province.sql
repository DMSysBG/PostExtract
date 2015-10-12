-- Function: temp_recognize_location_province()

-- DROP FUNCTION temp_recognize_location_province();

CREATE OR REPLACE FUNCTION temp_recognize_location_province()
  RETURNS integer AS
$BODY$
BEGIN
  -- Разпознава областите
   UPDATE n_template_location
      SET n_template_province_id = tp.id

   FROM n_template_location_item tli
      , n_template_province tp

   WHERE n_template_location.n_template_province_id IS NULL	-- Неразпознати области
     AND tli.n_template_location_id = n_template_location.id
     AND tli.t_location_type_id = 1
								-- Търси съответствия
     AND tp.t_location_type_id = tli.t_location_type_id
     AND lower(tp.tp_name) = lower(tli.t_location_name);
     
  RETURN 0;
END;
$BODY$
  LANGUAGE plpgsql VOLATILE
  COST 100;
ALTER FUNCTION temp_recognize_location_province()
  OWNER TO postgres;

-- Function: temp_load_location()

-- DROP FUNCTION temp_load_location();

CREATE OR REPLACE FUNCTION temp_load_location()
  RETURNS integer AS
$BODY$ 
BEGIN
  -- Добавя новите локации
  INSERT INTO n_template_location (n_template_id, post_location)
  SELECT DISTINCT np.n_template_id
       , np.post_location
  FROM new_post np
  LEFT JOIN n_template_location tl 
    ON tl.n_template_id = np.n_template_id
   AND tl.post_location = np.post_location
  WHERE tl.id IS NULL;

  -- Разпознава post_location
  UPDATE new_post 
     SET n_template_location_id = tl.id
  FROM n_template_location tl
  WHERE new_post.n_template_id = tl.n_template_id
    AND new_post.post_location = tl.post_location;

  RETURN 0;
END;
$BODY$
  LANGUAGE plpgsql VOLATILE
  COST 100;
ALTER FUNCTION temp_load_location()
  OWNER TO postgres;
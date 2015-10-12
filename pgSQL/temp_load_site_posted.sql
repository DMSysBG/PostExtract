-- Function: temp_load_site_posted()

-- DROP FUNCTION temp_load_site_posted();

CREATE OR REPLACE FUNCTION temp_load_site_posted()
  RETURNS integer AS
$BODY$ 
BEGIN
   -- Добавя новите публикатори
  INSERT INTO n_site_posted (n_site_id, sp_name)
  SELECT DISTINCT s.n_site_id
       , np.post_posted
  FROM new_post np
  LEFT JOIN n_source s
    ON s.id = np.n_source_id
  LEFT JOIN n_site_posted sp
    ON sp.n_site_id = s.n_site_id
   AND sp.sp_name = np.post_posted
  WHERE sp.id IS NULL;

  -- Разпознава публикаторите
  UPDATE new_post 
     SET n_site_posted_id = sp.id
  FROM n_source s
      , n_site_posted sp
  WHERE s.id = new_post.n_source_id
    AND sp.n_site_id = s.n_site_id
    AND sp.sp_name = new_post.post_posted;

  RETURN 0;
END;
$BODY$
  LANGUAGE plpgsql VOLATILE
  COST 100;
ALTER FUNCTION temp_load_site_posted()
  OWNER TO postgres;
-- Function: transfer_new_post()

-- DROP FUNCTION transfer_new_post();

CREATE OR REPLACE FUNCTION transfer_new_post()
  RETURNS integer AS
$BODY$
DECLARE
   postTransactionId integer;
BEGIN
   -- id на следващия трансфер
   postTransactionId := (SELECT coalesce(MAX(new_post_transaction_id), 0) FROM post) + 1;
   -- добавя публикациите
   INSERT INTO post
	( n_site_id
	, n_category_id
	, post_link
	, post_image
	, post_title
	, post_text
	, new_post_id
	, new_post_transaction_id
	, post_price
	, n_template_location_id
	, post_date
	, n_site_posted_id
	, post_price_type_id
	, post_progress_id )
   SELECT s.n_site_id
	, s.n_category_id
	, np.post_link
	, np.post_image
	, np.post_title
	, np.post_text
	, np.id AS new_post_id
	, postTransactionId AS new_post_transaction_id
	, CASE WHEN np.post_price~E'^\\d+' THEN np.post_price::decimal(12,2) ELSE 0 END AS post_price
	, np.n_template_location_id
	, np.post_date_time
	, np.n_site_posted_id
	, np.post_price_type_id
	, 1 AS post_progress_id
   FROM new_post np
   INNER JOIN n_source s ON s.id = np.n_source_id;

   -- добавя снимките
   INSERT INTO post_image
	( post_id
	, img_src )
   SELECT p.id
	, npi.img_src
   FROM post p
   INNER JOIN new_post_image npi ON npi.new_post_id = p.new_post_id
   WHERE p.new_post_transaction_id = postTransactionId;

   -- добавя атрибутите
   INSERT INTO post_attribute
	( post_id
	, pa_value
	, n_template_attribute_id
	, post_measure_id
	, n_template_attribute_item_id
	, n_category_attribute_type_id )
   SELECT p.id AS post_id
	, npa.attribute_value AS pa_value
	, npa.n_template_attribute_id
	, npa.post_measure_id
	, npa.n_template_attribute_item_id
	, npa.n_category_attribute_type_id
   FROM post p
   INNER JOIN new_post_attribute npa ON npa.new_post_id = p.new_post_id
     AND npa.n_category_attribute_type_id > 0
   WHERE p.new_post_transaction_id = postTransactionId;

   -- Маркира пуб. като обработена
   UPDATE post
      SET post_progress_id = 2
   WHERE new_post_transaction_id = postTransactionId;
   
   RETURN postTransactionId;
END;
$BODY$
  LANGUAGE plpgsql VOLATILE
  COST 100;
ALTER FUNCTION transfer_new_post()
  OWNER TO postgres;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using DMSys.Systems;
using DMSys.Data;
using Post.Models;

namespace PostExtract
{
    public class DBExtract : NpgsqlUtility
    {
        public DBExtract()
            : base(xConfig.ConnectionString)
        { }

        public PExtractTemplate GetPETemplate(int sourceId)
        {
            PExtractTemplate peTemplate = null;
            string commandText = String.Format(
@"SELECT s.id AS n_source_id
       , s.s_link
       , s.n_site_id
       , s.n_category_id
       , t.id AS n_template_id
       , t.t_name
       , t.xp_list
       , t.xp_list_link
       , t.xp_list_image
       , t.xp_post
       , t.xp_post_title
       , t.xp_post_text
       , t.xp_post_images
       , t.xp_post_attributes
       , t.sql_post_attributes
       , t.xp_post_price
       , t.xp_post_location
       , t.xp_post_date
       , t.xp_post_posted
FROM n_source s
INNER JOIN n_template t ON t.id = s.n_template_id
WHERE s.id = {0} ", SQLInt(sourceId));

            using (DataTable dtSource = base.FillDataTable(commandText))
            {
                if (dtSource.Rows.Count > 0)
                {
                    peTemplate = CreatePETemplate(dtSource.Rows[0]);
                }
            }
            return peTemplate;
        }

        public List<PExtractTemplate> GetPETemplates()
        {
            List<PExtractTemplate> peTemplates = new List<PExtractTemplate>();

            string commandText =
@"SELECT s.id AS n_source_id
       , s.s_link
       , s.n_site_id
       , s.n_category_id
       , t.id AS n_template_id
       , t.t_name
       , t.xp_list
       , t.xp_list_link
       , t.xp_list_image
       , t.xp_post
       , t.xp_post_title
       , t.xp_post_text
       , t.xp_post_images
       , t.xp_post_attributes
       , t.sql_post_attributes
       , t.xp_post_price
       , t.xp_post_location
       , t.xp_post_date
       , t.xp_post_posted
FROM n_source s
INNER JOIN n_template t ON t.id = s.n_template_id
WHERE s.is_active = 1 ";

            using (DataTable dtSource = base.FillDataTable(commandText))
            {
                foreach (DataRow drSource in dtSource.Rows)
                {
                    peTemplates.Add(CreatePETemplate(drSource));
                }
            }
            return peTemplates;
        }

        private PExtractTemplate CreatePETemplate(DataRow drSource)
        {
            PExtractTemplate peTemplate = new PExtractTemplate()
            {
                SourceId = TryParse.ToInt32(drSource["n_source_id"]),
                CategoryId = TryParse.ToInt32(drSource["n_category_id"]),
                TemplateId = TryParse.ToInt32(drSource["n_template_id"]),
                SiteUrl = TryParse.ToString(drSource["s_link"]),
                XPList = TryParse.ToString(drSource["xp_list"]),
                XPLink = TryParse.ToString(drSource["xp_list_link"]),
                XPImage = TryParse.ToString(drSource["xp_list_image"]),
                XPContent = TryParse.ToString(drSource["xp_post"]),
                XPTitle = TryParse.ToString(drSource["xp_post_title"]),
                XPText = TryParse.ToString(drSource["xp_post_text"]),
                XPImages = TryParse.ToString(drSource["xp_post_images"]),
                XPAttributes = TryParse.ToString(drSource["xp_post_attributes"]),
                SQLAttributes = TryParse.ToString(drSource["sql_post_attributes"]),
                XPPrice = TryParse.ToString(drSource["xp_post_price"]),
                XPLocation = TryParse.ToString(drSource["xp_post_location"]),
                XPDate = TryParse.ToString(drSource["xp_post_date"]),
                XPPosted = TryParse.ToString(drSource["xp_post_posted"])
            };
            return peTemplate;
        }

        public void EmptyNewPost()
        {
            string commandText =
@"DELETE FROM new_post_image;
  DELETE FROM new_post_attribute;
  DELETE FROM new_post;
  ALTER SEQUENCE new_post_image_id_seq RESTART WITH 1;
  ALTER SEQUENCE new_post_attribute_id_seq RESTART WITH 1;
  ALTER SEQUENCE new_post_id_seq RESTART WITH 1; ";

            base.ExecuteNonQuery(commandText);
        }

        public int AddNewPost(int sourceId, int categoryId, int templateId, string link, string image)
        {
            // Записва данните
            string commandText = String.Format(
@"INSERT INTO new_post ( n_source_id, n_category_id, n_template_id, post_link, post_image)
  VALUES ( {0}, {1}, {2}, '{3}', '{4}')
  RETURNING id; ", sourceId, categoryId, templateId, link, image);

            int rowId = TryParse.ToInt32(base.ExecuteScalar(commandText));
            return rowId;
        }

        public DataTable GetNewPost()
        {
            string commandText =
                "SELECT id, post_link FROM new_post";
            return base.FillDataTable(commandText);
        }

        /// <summary>
        /// Записва публикацията
        /// </summary>
        public void SaveNewPost(int postId, int categoryId, int templateId, PPost pPost)
        {
            string commandText = String.Format(
@"UPDATE new_post
     SET post_title = {1}
       , post_text = {2}
       , post_price = {3}
       , post_location = {4}
       , post_date = {5}
       , post_posted = {6}
  WHERE id = {0}", SQLInt(postId)
                 , SQLString(pPost.Title)
                 , SQLString(pPost.Text)
                 , SQLString(pPost.Price)
                 , SQLString(pPost.Location)
                 , SQLString(pPost.Date)
                 , SQLString(pPost.Posted));

            base.ExecuteNonQuery(commandText);

            foreach (string imgSrc in pPost.Images)
            {
                commandText = String.Format(
@"INSERT INTO new_post_image ( new_post_id, img_src )
VALUES ( {0}, {1} ) ", SQLInt(postId), SQLString(imgSrc));

                base.ExecuteNonQuery(commandText);
            }

            foreach (string attribute in pPost.Attributes)
            {
                commandText = String.Format(
@"INSERT INTO new_post_attribute ( new_post_id, attribute_value, n_category_id, n_template_id )
VALUES ( {0}, {1}, {2}, {3}) ", SQLInt(postId), SQLString(attribute), SQLInt(categoryId), SQLInt(templateId));

                base.ExecuteNonQuery(commandText);
            }
        }

        /// <summary>
        /// Прехвърля новите публикации
        /// </summary>
        public void TransferNewPost()
        {
             string commandText =
                    "SELECT transfer_new_post()";
             base.ExecuteScalar(commandText);
        }
        
        /// <summary>
        /// Премахва дублираните
        /// </summary>
        public void RemovesDuplicate()
        {
            string commandText =
@"DELETE FROM new_post
WHERE id in ( SELECT np.id FROM new_post np
		      INNER JOIN post p ON p.post_link = np.post_link ) ";
            base.ExecuteNonQuery(commandText);

            commandText =
@"DELETE FROM new_post_attribute
WHERE new_post_id NOT IN (SELECT id FROM new_post) ";
            base.ExecuteNonQuery(commandText);

            commandText =
@"DELETE FROM new_post_image
WHERE new_post_id NOT IN (SELECT id FROM new_post) ";
            base.ExecuteNonQuery(commandText);
        }

        /// <summary>
        /// Валидира публикациите
        /// </summary>
        public void ValidatePublications()
        {
            // локации
            string commandText = "SELECT temp_load_location();";
            base.ExecuteScalar(commandText);

            // публикатори
            commandText = "SELECT temp_load_site_posted();";
            base.ExecuteScalar(commandText);

            // местоположения
            commandText = "SELECT temp_repair_location_item();";
            base.ExecuteScalar(commandText);

            // атрибути
            commandText = "SELECT temp_load_attribute_item();";
            base.ExecuteScalar(commandText);

            // Записва в лога публ. с неразпоснати атрибути
            commandText =
@"INSERT INTO sys_exception (n_source_id, post_link, ex_message, stack_trace, ex_date)
 SELECT DISTINCT np.n_source_id
	  , np.post_link
	  , 'attribute: '''||npa.attribute_value||'''' AS stack_trace
	  , 'Невалиден атрибут' AS ex_message
      , now() AS ex_date
 FROM new_post np
 LEFT JOIN new_post_attribute npa ON npa.new_post_id = np.id
 LEFT JOIN n_template_attribute ta ON ta.id = npa.n_template_attribute_id
 LEFT JOIN n_category_attribute pa ON pa.id = ta.n_category_attribute_id
 WHERE pa.id IS NULL ";
            base.ExecuteNonQuery(commandText);

            // Премахва атрибутите без публикация
            commandText =
@"DELETE FROM new_post_attribute
WHERE new_post_id NOT IN (SELECT id FROM new_post) ";
            base.ExecuteNonQuery(commandText);

            // Премахва снимките без публикация
            commandText =
@"DELETE FROM new_post_image
WHERE new_post_id NOT IN (SELECT id FROM new_post) ";
            base.ExecuteNonQuery(commandText);
        }

        /// <summary>
        /// Публикация
        /// </summary>
        public PostModel GetPost(int postId)
        {
            string commandText =
@"SELECT p.id
       , p.n_site_id
       , p.n_category_id
       , p.post_link
       , p.post_image
       , p.post_title
       , p.post_text
       , p.new_post_id
       , p.new_post_transaction_id
       , p.post_price
       , p.n_template_location_id
       , p.post_date
       , p.n_site_posted_id
       , p.post_price_type_id
 FROM post p
 WHERE p.id = " + SQLInt(postId);
            PostModel model = null;
            using (DataTable dtPost = base.FillDataTable(commandText))
            {
                if (dtPost.Rows.Count > 0)
                {
                    DataRow drPost = dtPost.Rows[0];
                    model = new PostModel()
                    {
                        PostId = TryParse.ToInt32(drPost["id"]),
                        SiteId = TryParse.ToInt32(drPost["n_site_id"]),
                        CategoryId = TryParse.ToInt32(drPost["n_category_id"]),
                        PLink = TryParse.ToString(drPost["post_link"]),
                        PImage = TryParse.ToString(drPost["post_image"]),
                        PTitle = TryParse.ToString(drPost["post_title"]),
                        PText = TryParse.ToString(drPost["post_text"]),
                        PPrice = TryParse.ToDecimal(drPost["post_price"]),
                        PDate = TryParse.ToDateTime(drPost["post_date"]),
                        SitePostedId = TryParse.ToInt32(drPost["n_site_posted_id"]),
                        PPriceTypeId = TryParse.ToInt32(drPost["post_price_type_id"])
                    };
                }
            }
            return model;
        }
    }
}

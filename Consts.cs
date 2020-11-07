namespace QuizHub.Server {
    public static class Consts {
        public const int COOLDOWN_DURATION = 24 * 60 * 60;
        
        public const string NAME = "@Name";
        public const string ID = "@Id";
        public const string IMG_URL = "@ImgUrl";
        public const string VK_ID = "@VkId";
        public const string CATEGORY_ID = "@CategoryId";
        public const string TITLE = "@Title";
        public const string DIFFICULTY = "@Difficulty";
        public const string QUIZ_ID = "@QuizId";
        public const string TEXT = "@Text";
        public const string POINTS = "@Points";
        public const string OPTIONS = "@Options";
        public const string ANSWER = "@Answer";
        public const string RUN_ID = "@RunId";
        public const string QUESTION_ID = "@QuestionId";
        public const string IS_IT_RIGHT = "@IsItRight";
        public const string BALANCE_DELTA = "@BalanceDelta";
        public const string BALANCE = "@Balance";
        public const string URL = "@Url";
        public const string SHOP_ID = "@ShopId";
        public const string BOOK_ID = "@BookId";
        public const string PRICE = "@Price";
        public const string DISCOUNT_ID = "@DiscountId";
        public const string COUPON = "@Coupon";
        public const string COUPON_ID = "@CouponId";
        public const string DESCRIPTION = "@Description";

        public const string SELECT_CATEGORIES = "SELECT id, name, img_url FROM categories;";
        public const string SELECT_CATEGORY_BY_ID = "SELECT id, name, img_url FROM categories WHERE id = @Id LIMIT 1;";
        public static readonly string SELECT_QUIZZES = $"SELECT id, title, difficulty, (SELECT JSON_OBJECT('id', r.id, 'runAt', UNIX_TIMESTAMP(r.run_at), 'cooldownAt', UNIX_TIMESTAMP(r.run_at) + {COOLDOWN_DURATION}, 'questionCount', (SELECT COUNT(*) FROM questions qu WHERE qu.quiz_id = q.id), 'rightAnswerCount', IFNULL((SELECT SUM(is_it_right) FROM answers a WHERE a.run_id = r.id), 0)) FROM runs r WHERE r.quiz_id = q.id and r.vk_id = @VkId ORDER BY id DESC LIMIT 1) FROM quizes q WHERE category_id = @CategoryId;";
        public const string SELECT_LAST_RUN = "SELECT id, vk_id, quiz_id, run_at FROM runs WHERE quiz_id = @QuizId AND vk_id = @VkId ORDER BY id DESC LIMIT 1;";
        public const string SELECT_QUESTIONS = "SELECT id, text, points, options, answer, type FROM questions WHERE quiz_id = @QuizId;";
        public const string SELECT_ANSWERS_FROM_QUESTIONS = "SELECT id, points, type, answer, IFNULL((SELECT MAX(a.is_it_right) FROM answers a JOIN runs r ON a.run_id = r.id WHERE r.quiz_id = q.quiz_id AND r.vk_id = @VkId AND a.question_id = q.id), FALSE) FROM questions q WHERE q.quiz_id = @QuizId;";
        public const string SELECT_USER_STAT = "SELECT IFNULL(SUM(YEAR(r.run_at) = YEAR(NOW()) AND MONTH(r.run_at) = MONTH(NOW())), 0), COUNT(*), (SELECT t.name FROM (SELECT c.name name, COUNT(*) AS run_count FROM runs r JOIN quizes q ON r.quiz_id = q.id JOIN categories c ON q.category_id = c.id WHERE r.vk_id = @VkId GROUP BY c.id ORDER BY run_count DESC LIMIT 1) t), IFNULL((SELECT balance FROM balances b WHERE b.vk_id = @VkId LIMIT 1), 0) FROM runs r WHERE r.vk_id = @VkId;";
        public const string SELECT_BOOK_BY_ID = "SELECT b.id, b.title, b.description, b.url, b.img_url, s.id, s.name, s.img_url, s.url FROM books b JOIN shops s ON b.shop_id = s.id WHERE b.id = @BookId LIMIT 1;";
        public const string SELECT_BOOK_ASSIGNMENTS_BY_QUIZ_ID = "SELECT b.id, b.title, b.description, b.url, b.img_url, s.id, s.name, s.img_url, s.url FROM map_book_quiz mbq JOIN books b ON b.id = mbq.book_id JOIN shops s ON b.shop_id = s.id WHERE mbq.quiz_id = @QuizId;";
        public const string SELECT_DISCOUNT_BY_ID = "SELECT title, price, s.id, name, img_url, url FROM discounts d JOIN shops s ON d.shop_id = s.id WHERE d.id = @DiscountId LIMIT 1;";
        public const string SELECT_DISCOUNTS = "SELECT d.id, title, price, s.id, name, img_url, url FROM discounts d JOIN shops s ON d.shop_id = s.id;";
        public const string SELECT_BALANCE = "SELECT balance FROM balances WHERE vk_id = @VkId LIMIT 1;";
        public const string SELECT_COUPON_BY_ID = "SELECT c.id, coupon, created_at, d.id, title, price, s.id, name, img_url, url FROM coupons c JOIN discounts d ON c.discount_id = d.id JOIN shops s ON d.shop_id = s.id WHERE c.id = @CouponId AND !c.is_used LIMIT 1;";
        public const string SELECT_COUPONS = "SELECT c.id, coupon, created_at, d.id, title, price, s.id, name, img_url, url FROM coupons c JOIN discounts d ON c.discount_id = d.id JOIN shops s ON d.shop_id = s.id WHERE c.vk_id = @VkId;";
        public const string SELECT_SHOP_BY_ID = "SELECT id, name, img_url, url FROM shops WHERE id = @ShopId LIMIT 1;";

        public const string INSERT_NEW_CATEGORY = "INSERT INTO categories(name) VALUES (@Name);";
        public const string INSERT_NEW_QUIZ = "INSERT INTO quizes(title, category_id, difficulty) VALUES (@Title, @CategoryId, @Difficulty);";
        public const string INSERT_NEW_QUESTION = "INSERT INTO questions(quiz_id, text, points, options, answer) VALUES(@QuizId, @Text, @Points, @Options, @Answer);";
        public const string INSERT_NEW_RUN = "INSERT INTO runs(vk_id, quiz_id) VALUES(@VkId, @QuizId);";
        public const string INSERT_NEW_ANSWER = "INSERT INTO answers(run_id, answer, question_id, is_it_right) VALUES(@RunId, @Answer, @QuestionId, @IsItRight);";
        public const string INSERT_NEW_BALANCE = "INSERT INTO balances(vk_id, balance) VALUES(@VkId, @Balance);";
        public const string INSERT_NEW_SHOP = "INSERT INTO shops(name, url) VALUES(@Name, @Url);";
        public const string INSERT_NEW_BOOK = "INSERT INTO books(shop_id, title, url) VALUES(@ShopId, @Title, @Url);";
        public const string INSERT_NEW_MAP_BOOK_QUIZ = "INSERT IGNORE INTO map_book_quiz(book_id, quiz_id) VALUES(@BookId, @QuizId);";
        public const string INSERT_NEW_DISCOUNT = "INSERT INTO discounts(shop_id, title, price) VALUES(@ShopId, @Title, @Price);";
        public const string INSERT_NEW_COUPON = "INSERT INTO coupons(vk_id, discount_id, coupon) VALUES(@VkId, @DiscountId, @Coupon);";

        public const string UPDATE_CATEGORY = "UPDATE categories SET {0} WHERE id = @Id;";
        public const string UPDATE_BALANCE = "UPDATE balances SET balance = balance + @BalanceDelta WHERE vk_id = @VkId LIMIT 1;";
        public const string UPDATE_SHOP = "UPDATE shops SET {0} WHERE id = @Id;";
        public const string UPDATE_BOOK = "UPDATE books SET {0} WHERE id = @Id;";
    }
}
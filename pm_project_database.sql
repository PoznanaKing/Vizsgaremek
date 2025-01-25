-- phpMyAdmin SQL Dump
-- version 5.2.1
-- https://www.phpmyadmin.net/
--
-- Gép: 127.0.0.1
-- Létrehozás ideje: 2025. Jan 25. 10:51
-- Kiszolgáló verziója: 10.4.32-MariaDB
-- PHP verzió: 8.2.12

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Adatbázis: `pm_project_database`
--
CREATE DATABASE IF NOT EXISTS `pm_project_database` DEFAULT CHARACTER SET utf8 COLLATE utf8_hungarian_ci;
USE `pm_project_database`;

-- --------------------------------------------------------

--
-- Tábla szerkezet ehhez a táblához `message_content`
--

CREATE TABLE `message_content` (
  `id` char(36) NOT NULL,
  `message_sender_id` char(36) NOT NULL,
  `message_content` char(200) NOT NULL,
  `message_sent_time` datetime DEFAULT NULL,
  `chat_id` char(36) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_hungarian_ci;

-- --------------------------------------------------------

--
-- Tábla szerkezet ehhez a táblához `place_owner_table`
--

CREATE TABLE `place_owner_table` (
  `owner_id` char(36) NOT NULL,
  `owner_name` char(40) NOT NULL,
  `owner_email` char(40) NOT NULL,
  `owner_password` char(40) NOT NULL,
  `verified` tinyint(1) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_hungarian_ci;

-- --------------------------------------------------------

--
-- Tábla szerkezet ehhez a táblához `place_table`
--

CREATE TABLE `place_table` (
  `placeid` char(36) NOT NULL,
  `place_name` char(40) NOT NULL,
  `postal_code` int(11) NOT NULL,
  `town_name` char(40) NOT NULL,
  `street_name` char(50) NOT NULL,
  `story_levle` int(11) DEFAULT NULL,
  `description` char(200) DEFAULT NULL,
  `rating` double DEFAULT NULL,
  `trainer_id` char(36) DEFAULT NULL,
  `owner_id` char(36) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_hungarian_ci;

-- --------------------------------------------------------

--
-- Tábla szerkezet ehhez a táblához `place_user_connector`
--

CREATE TABLE `place_user_connector` (
  `userid` char(36) DEFAULT NULL,
  `placeid` char(36) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_hungarian_ci;

-- --------------------------------------------------------

--
-- Tábla szerkezet ehhez a táblához `post_table`
--

CREATE TABLE `post_table` (
  `postId` char(36) NOT NULL,
  `postTitle` char(40) NOT NULL,
  `postDescription` char(100) DEFAULT NULL,
  `postImage` longblob DEFAULT NULL,
  `userid` char(36) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_hungarian_ci;

-- --------------------------------------------------------

--
-- Tábla szerkezet ehhez a táblához `trainer_table`
--

CREATE TABLE `trainer_table` (
  `trainer_id` char(36) NOT NULL,
  `trainer_name` char(40) NOT NULL,
  `trainer_email` char(40) NOT NULL,
  `trainer_password` char(40) NOT NULL,
  `verified` tinyint(1) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_hungarian_ci;

--
-- A tábla adatainak kiíratása `trainer_table`
--

INSERT INTO `trainer_table` (`trainer_id`, `trainer_name`, `trainer_email`, `trainer_password`, `verified`) VALUES
('dcf57e1a-2bb0-4279-baf3-9ffb978a6828', 'János Péter', 'janipeti@gmail.com', 'asdasd11', NULL);

-- --------------------------------------------------------

--
-- Tábla szerkezet ehhez a táblához `trainer_user_message_connector`
--

CREATE TABLE `trainer_user_message_connector` (
  `chat_id` char(36) NOT NULL,
  `user_id` char(36) NOT NULL,
  `trainer_id` char(36) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_hungarian_ci;

-- --------------------------------------------------------

--
-- Tábla szerkezet ehhez a táblához `user_table`
--

CREATE TABLE `user_table` (
  `id` char(36) NOT NULL,
  `username` char(40) NOT NULL,
  `userpassword` char(40) NOT NULL,
  `email` char(40) NOT NULL,
  `verified` tinyint(1) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_hungarian_ci;

--
-- A tábla adatainak kiíratása `user_table`
--

INSERT INTO `user_table` (`id`, `username`, `userpassword`, `email`, `verified`) VALUES
('0a78fb9b-bbbf-4a9b-a0ff-444697b644bf', 'string', 'string', 'string', 0),
('65f3ac8f-966a-490d-9378-0ff1cc5863c5', 'string', 'string', 'string', 0),
('c53fe4a5-87ea-4acf-9fbe-e89216776d8e', 'string', 'string', 'string', 0),
('f317fc87-b95b-4090-97d4-8f6751c83bef', 'string', 'string', 'string', 0);

--
-- Indexek a kiírt táblákhoz
--

--
-- A tábla indexei `message_content`
--
ALTER TABLE `message_content`
  ADD PRIMARY KEY (`id`),
  ADD KEY `chat_id` (`chat_id`);

--
-- A tábla indexei `place_owner_table`
--
ALTER TABLE `place_owner_table`
  ADD PRIMARY KEY (`owner_id`);

--
-- A tábla indexei `place_table`
--
ALTER TABLE `place_table`
  ADD PRIMARY KEY (`placeid`),
  ADD KEY `trainer_id` (`trainer_id`,`owner_id`),
  ADD KEY `owner_id` (`owner_id`);

--
-- A tábla indexei `place_user_connector`
--
ALTER TABLE `place_user_connector`
  ADD KEY `userid` (`userid`,`placeid`),
  ADD KEY `placeid` (`placeid`);

--
-- A tábla indexei `post_table`
--
ALTER TABLE `post_table`
  ADD PRIMARY KEY (`postId`),
  ADD KEY `userid` (`userid`);

--
-- A tábla indexei `trainer_table`
--
ALTER TABLE `trainer_table`
  ADD PRIMARY KEY (`trainer_id`);

--
-- A tábla indexei `trainer_user_message_connector`
--
ALTER TABLE `trainer_user_message_connector`
  ADD PRIMARY KEY (`chat_id`),
  ADD KEY `user_id` (`user_id`,`trainer_id`),
  ADD KEY `trainer_id` (`trainer_id`);

--
-- A tábla indexei `user_table`
--
ALTER TABLE `user_table`
  ADD PRIMARY KEY (`id`);

--
-- Megkötések a kiírt táblákhoz
--

--
-- Megkötések a táblához `message_content`
--
ALTER TABLE `message_content`
  ADD CONSTRAINT `message_content_ibfk_1` FOREIGN KEY (`chat_id`) REFERENCES `trainer_user_message_connector` (`chat_id`) ON DELETE CASCADE;

--
-- Megkötések a táblához `place_table`
--
ALTER TABLE `place_table`
  ADD CONSTRAINT `place_table_ibfk_1` FOREIGN KEY (`owner_id`) REFERENCES `place_owner_table` (`owner_id`) ON DELETE CASCADE,
  ADD CONSTRAINT `place_table_ibfk_2` FOREIGN KEY (`trainer_id`) REFERENCES `trainer_table` (`trainer_id`);

--
-- Megkötések a táblához `place_user_connector`
--
ALTER TABLE `place_user_connector`
  ADD CONSTRAINT `place_user_connector_ibfk_1` FOREIGN KEY (`userid`) REFERENCES `user_table` (`id`),
  ADD CONSTRAINT `place_user_connector_ibfk_2` FOREIGN KEY (`placeid`) REFERENCES `place_table` (`placeid`);

--
-- Megkötések a táblához `post_table`
--
ALTER TABLE `post_table`
  ADD CONSTRAINT `post_table_ibfk_1` FOREIGN KEY (`userid`) REFERENCES `user_table` (`id`) ON DELETE CASCADE;

--
-- Megkötések a táblához `trainer_user_message_connector`
--
ALTER TABLE `trainer_user_message_connector`
  ADD CONSTRAINT `trainer_user_message_connector_ibfk_1` FOREIGN KEY (`user_id`) REFERENCES `user_table` (`id`),
  ADD CONSTRAINT `trainer_user_message_connector_ibfk_2` FOREIGN KEY (`trainer_id`) REFERENCES `trainer_table` (`trainer_id`);
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;

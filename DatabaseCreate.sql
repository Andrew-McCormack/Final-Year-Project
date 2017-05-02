SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
SET time_zone = "+00:00";

drop table if exists WordResponse;
drop table if exists Word;

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8 */;

--
-- Database: `WordFrequencyLookup`
--

-- --------------------------------------------------------

--
-- Table structure for table `WordResponse`
--

DROP TABLE IF EXISTS `WordResponse`;
DROP TABLE IF EXISTS `Word`;

CREATE TABLE `WordResponse` (
  `wordResponseId` int(20) NOT NULL AUTO_INCREMENT,
  `wordId` int(15) NOT NULL,
  `wordResponse` varchar(21804) CHARACTER SET utf8 NOT NULL,
  `weight` float(15) NOT NULL,
  `previousSubSize` int(3) NOT NULL,
  `responseGroupId` int(15) NOT NULL,
  PRIMARY KEY (wordResponseId)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

--
-- Table structure for table `Word`
--

CREATE TABLE  `Word` (
  `wordId` int(15) NOT NULL AUTO_INCREMENT,
  `word` varchar(15),
  PRIMARY KEY (wordId)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;


--
-- Constraints for dumped tables
--

--
-- Constraints for table `WordResponse`
--
ALTER TABLE `WordResponse`
ADD CONSTRAINT `fk_word_wordResponse_wordId` FOREIGN KEY (`wordId`) REFERENCES `Word` (`wordId`) ON DELETE CASCADE ON UPDATE CASCADE;
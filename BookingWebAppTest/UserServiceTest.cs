using Interfaces;
using Models.Entities;
using Moq;
using Services;

namespace BookingWebAppTest;

[TestClass]
public class UserServiceTest
{
    private UserService _userService;
    private IPasswordSecurityService _passwordSecurityService;
    private Mock<IUserRepository> _mockUserRepository;



    [TestInitialize]
    public void Setup()
    {
        _mockUserRepository = new Mock<IUserRepository>();
        _passwordSecurityService = new PasswordSecurityService();
      _userService = new UserService(_mockUserRepository.Object,_passwordSecurityService);
    }

    [TestMethod]
    public void GetExistedLogIn_ShouldReturnZero_WhenEqualsToMinusOne() // bad flow
    {
        User user = new User(-1);
        var expectedResult = 0;
        _mockUserRepository.Setup(repo => repo.GetUser(It.IsAny<string>())).Returns(user);
        var userInt = _userService.GetExistedLogIn("something@mail.com", "1234567890");
        Assert.AreEqual(expectedResult,userInt);
    }

    [TestMethod]
    public void GetExistedLogIn_ShouldReturnAValidUserId_WhenUserExist()
    {
        var user = new User(12,"something@mail.com","1234567890","joshua", "dAmSQ8dJ8hQSRCSjAKFdRR54JHXZ+tVUJJCIdLZkTQtxFY0XM1CyNtLw+BB+5/cNIxTRFQSCJU8oDcR3vQIGWw==", 1);
        _mockUserRepository.Setup(repo => repo.GetUser(It.IsAny<string>())).Returns(user);
        _mockUserRepository.Setup(repo => repo.LogIn(It.IsAny<string>(),It.IsAny<string>())).Returns(user.Id);
        var userInt = _userService.GetExistedLogIn("something@mail.com", "1234567890");
        var expectedResult = user.Id;
        Assert.AreEqual(expectedResult,userInt);
    }


}

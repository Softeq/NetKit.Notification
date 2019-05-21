// Developed by Softeq Development Corporation
// http://www.softeq.com

using Moq;
using Softeq.NetKit.Notifications.Domain.DataStores;
using Softeq.NetKit.Notifications.Domain.Exceptions;
using Softeq.NetKit.Notifications.Domain.Models.Localization;
using Softeq.NetKit.Notifications.Domain.Models.Notification;
using Softeq.NetKit.Notifications.Domain.Models.NotificationSettings;
using Softeq.NetKit.Notifications.Service.NotificationSenders;
using Softeq.NetKit.Notifications.Service.Services;
using Softeq.NetKit.Notifications.Service.TransportModels.Settings;
using Softeq.NetKit.Notifications.Service.TransportModels.Settings.Request;
using Softeq.NetKit.Notifications.Service.TransportModels.Settings.Response;
using Softeq.NetKit.Notifications.Service.TransportModels.Shared.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Softeq.NetKit.Notifications.Service.Tests.ServiceTests
{
    public class SettingsServiceTests : UnitTestBase
    {
        [Fact]
        [Trait("Category", "Unit")]
        public async Task ValidationFailedTest()
        {
            var store = new Mock<ISettingsDataStore>();

            var service = new SettingsService(store.Object, DefaultMapper);
            await Assert.ThrowsAsync<ArgumentNullException>(() => service.CreateUserSettingsAsync(null));
            await Assert.ThrowsAsync<ArgumentNullException>(() => service.UpdateUserSettingsAsync(null));
            await Assert.ThrowsAsync<ArgumentNullException>(() => service.GetUserSettingsAsync(null));
            await Assert.ThrowsAsync<ArgumentNullException>(() => service.GetNotificationSettingsAsync(null));
            await Assert.ThrowsAsync<ArgumentNullException>(() => service.UpdateNotificationSettingsAsync(null));

            store.Verify(x => x.FindAsync(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async Task WhenCreatingUserSettingsForExistingUserThenConflictThrownTest()
        {
            var userId = Guid.NewGuid().ToString();

            var store = new Mock<ISettingsDataStore>();
            store.Setup(x => x.DoesExistAsync(It.Is<string>(s => s == userId))).ReturnsAsync(true);

            var service = new SettingsService(store.Object, DefaultMapper);
            await Assert.ThrowsAsync<ConflictException>(() => service.CreateUserSettingsAsync(new UserProfileRequest
            {
                UserId = userId
            }));

            store.Verify(x => x.SaveAsync(It.IsAny<UserSettings>()), Times.Never);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async Task CreateUserSettingsSuccessfullyTest()
        {
            var userId = Guid.NewGuid().ToString();
            var request = new UserProfileRequest
            {
                Email = "alex@softeq.com",
                FirstName = "Alex",
                Language = LanguageName.Fr,
                LastName = "Softeq",
                PhoneNumber = "123",
                UserId = userId
            };

            UserSettings newSettings = null;
            var store = new Mock<ISettingsDataStore>();
            store.Setup(x => x.DoesExistAsync(It.Is<string>(s => s == userId))).ReturnsAsync(false);
            store.Setup(x => x.SaveAsync(It.IsAny<UserSettings>()))
                .Callback<UserSettings>(settings => newSettings = settings)
                .ReturnsAsync(() => newSettings);

            var service = new SettingsService(store.Object, DefaultMapper);
            var response = await service.CreateUserSettingsAsync(request);

            store.Verify(x => x.SaveAsync(It.IsAny<UserSettings>()), Times.Once);
            VerifyUserSettings(request, response);

            var flatten = NotificationEventConfiguration.Config
                .SelectMany(f => f.Value.Where(x => !x.IsMandatory).Select(s => (f.Key, s)))
                .ToList();

            Assert.Equal(flatten.Count, response.Settings.Count());

            foreach (var setting in flatten)
            {
                Assert.NotNull(response.Settings.FirstOrDefault(x => x.Enabled && x.Type == setting.Item1 && x.Event == setting.Item2.Event));
            }
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async Task WhenUpdatingSettingsAndProfileDoesNotExistThenExceptionThrownTest()
        {
            var userId = Guid.NewGuid().ToString();

            var store = new Mock<ISettingsDataStore>();
            store.Setup(x => x.FindAsync(It.Is<string>(s => s == userId))).ReturnsAsync((UserSettings)null);

            var service = new SettingsService(store.Object, DefaultMapper);
            await Assert.ThrowsAsync<NotFoundException>(() => service.UpdateUserSettingsAsync(new UserProfileRequest
            {
                UserId = userId
            }));

            store.Verify(x => x.UpdateAsync(It.IsAny<UserSettings>()), Times.Never);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async Task SuccessfulUserSettingsUpdateTest()
        {
            var userId = Guid.NewGuid().ToString();
            var currentSettings = new UserSettings
            {
                UserId = userId,
                LastName = "oldLast",
                FirstName = "oldFirst",
                Email = "oldEmail",
                PhoneNumber = "oldPhone",
                Language = LanguageName.En
            };

            var request = new UserProfileRequest
            {
                Email = "alex@softeq.com",
                FirstName = "Alex",
                Language = LanguageName.Fr,
                LastName = "Softeq",
                PhoneNumber = "123",
                UserId = userId
            };

            UserSettings newSettings = null;
            var store = new Mock<ISettingsDataStore>();
            store.Setup(x => x.FindAsync(It.Is<string>(s => s == userId))).ReturnsAsync(currentSettings);
            store.Setup(x => x.UpdateAsync(It.IsAny<UserSettings>()))
                .Callback<UserSettings>(settings => newSettings = settings)
                .ReturnsAsync(() => newSettings);

            var service = new SettingsService(store.Object, DefaultMapper);
            var response = await service.UpdateUserSettingsAsync(request);

            store.Verify(x => x.UpdateAsync(It.IsAny<UserSettings>()), Times.Once);
            VerifyUserSettings(request, response);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async Task WhenGettingSettingsAndProfileDoesNotExistThenExceptionThrownTest()
        {
            var userId = Guid.NewGuid().ToString();

            var store = new Mock<ISettingsDataStore>();
            store.Setup(x => x.FindAsync(It.Is<string>(s => s == userId))).ReturnsAsync((UserSettings)null);

            var service = new SettingsService(store.Object, DefaultMapper);
            await Assert.ThrowsAsync<NotFoundException>(() => service.GetUserSettingsAsync(new UserRequest(userId)));
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async Task SuccessfulUserSettingsRetrievalTest()
        {
            var userId = Guid.NewGuid().ToString();
            var currentSettings = new UserSettings
            {
                UserId = userId,
                LastName = "oldLast",
                FirstName = "oldFirst",
                Email = "oldEmail",
                PhoneNumber = "oldPhone",
                Language = LanguageName.En
            };

            var store = new Mock<ISettingsDataStore>();
            store.Setup(x => x.FindAsync(It.Is<string>(s => s == userId))).ReturnsAsync(currentSettings);

            var service = new SettingsService(store.Object, DefaultMapper);
            var response = await service.GetUserSettingsAsync(new UserRequest(userId));

            VerifyUserSettings(currentSettings, response);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async Task WhenGettingNotificationSettingsAndProfileDoesNotExistThenExceptionThrownTest()
        {
            var userId = Guid.NewGuid().ToString();

            var store = new Mock<ISettingsDataStore>();
            store.Setup(x => x.FindAsync(It.Is<string>(s => s == userId))).ReturnsAsync((UserSettings)null);

            var service = new SettingsService(store.Object, DefaultMapper);
            await Assert.ThrowsAsync<NotFoundException>(() => service.GetNotificationSettingsAsync(new UserRequest(userId)));
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async Task WhenUpdatingNotificationSettingsAndProfileDoesNotExistThenExceptionThrownTest()
        {
            var userId = Guid.NewGuid().ToString();

            var store = new Mock<ISettingsDataStore>();
            store.Setup(x => x.FindAsync(It.Is<string>(s => s == userId))).ReturnsAsync((UserSettings)null);

            var service = new SettingsService(store.Object, DefaultMapper);
            await Assert.ThrowsAsync<NotFoundException>(() => service.UpdateNotificationSettingsAsync(new UpdateNotificationSettingsRequest
            {
                UserId = userId,
                Settings = new List<NotificationSettingModel>
                {
                    new NotificationSettingModel
                    {
                        Type = NotificationType.Email,
                        Event =  NotificationEvent.ArticleCreated,
                        Enabled = true
                    }
                }
            }));
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async Task SuccessfulNotificationSettingsRetrievalTest()
        {
            var userId = Guid.NewGuid().ToString();
            var currentSettings = new UserSettings
            {
                UserId = userId,
                Settings = new List<NotificationSetting>
                {
                    new NotificationSetting(NotificationType.Push, NotificationEvent.ArticleCreated, true),
                    new NotificationSetting(NotificationType.Email, NotificationEvent.PackageArrived, true)
                }
            };

            var store = new Mock<ISettingsDataStore>();
            store.Setup(x => x.FindAsync(It.Is<string>(s => s == userId))).ReturnsAsync(currentSettings);

            var service = new SettingsService(store.Object, DefaultMapper);
            var response = await service.GetNotificationSettingsAsync(new UserRequest(userId));

            Assert.NotNull(response);
            Assert.Equal(userId, response.UserId);
            Assert.Equal(2, response.Settings.Count());
            foreach (var setting in currentSettings.Settings)
            {
                Assert.NotNull(response.Settings.FirstOrDefault(x => x.Enabled == setting.Enabled && x.Type == setting.Type && x.Event == setting.Event));
            }
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async Task SuccessfulNotificationSettingsUpdateTest()
        {
            var userId = Guid.NewGuid().ToString();
            var currentSettings = new UserSettings
            {
                UserId = userId,
                Settings = new List<NotificationSetting>
                {
                    new NotificationSetting(NotificationType.Push, NotificationEvent.ArticleCreated, true),
                    new NotificationSetting(NotificationType.Email, NotificationEvent.PackageArrived, true),
                    new NotificationSetting(NotificationType.SMS, NotificationEvent.SendSmsCode, true),
                }
            };

            var missingSettings = new List<NotificationSetting>
            {
                new NotificationSetting(NotificationType.SMS, NotificationEvent.SendSmsCode, true)
            };

            var request = new UpdateNotificationSettingsRequest
            {
                UserId = userId,
                Settings = new List<NotificationSettingModel>
                {
                    new NotificationSettingModel
                    {
                        Type = NotificationType.Push,
                        Event =  NotificationEvent.CommentLiked,
                        Enabled = true
                    },
                    new NotificationSettingModel
                    {
                        Type = NotificationType.Email,
                        Event =  NotificationEvent.PackageArrived,
                        Enabled = false
                    },
                    new NotificationSettingModel
                    {
                        Type = NotificationType.Push,
                        Event =  NotificationEvent.ArticleCreated,
                        Enabled = false
                    },
                    new NotificationSettingModel
                    {
                        Type = NotificationType.SMS,
                        Event =  NotificationEvent.SendSmsCode,
                        Enabled = true
                    }
                }
            };

            var store = new Mock<ISettingsDataStore>();
            UserSettings newSettings = null;
            store.Setup(x => x.FindAsync(It.Is<string>(s => s == userId))).ReturnsAsync(currentSettings);
            store.Setup(x => x.UpdateAsync(It.IsAny<UserSettings>()))
                .Callback<UserSettings>(settings => newSettings = settings)
                .ReturnsAsync(() => newSettings);


            var service = new SettingsService(store.Object, DefaultMapper);
            var response = await service.UpdateNotificationSettingsAsync(request);

            Assert.NotNull(response);
            Assert.Equal(userId, response.UserId);
            Assert.Equal(4, response.Settings.Count());
            foreach (var setting in request.Settings)
            {
                Assert.NotNull(response.Settings.FirstOrDefault(x => x.Enabled == setting.Enabled && x.Type == setting.Type && x.Event == setting.Event));
            }

            foreach (var setting in missingSettings)
            {
                Assert.NotNull(response.Settings.FirstOrDefault(x => x.Enabled == setting.Enabled && x.Type == setting.Type && x.Event == setting.Event));
            }
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async Task WhenUnsupportedNotificationSettingsProvidedThenValidationErrorRaisedTest()
        {
            var userId = Guid.NewGuid().ToString();
            var currentSettings = new UserSettings
            {
                UserId = userId,
                Settings = new List<NotificationSetting>
                {
                    new NotificationSetting(NotificationType.Push, NotificationEvent.ArticleCreated, true),
                    new NotificationSetting(NotificationType.Email, NotificationEvent.PackageArrived, true)
                }
            };

            var request = new UpdateNotificationSettingsRequest
            {
                UserId = userId,
                Settings = new List<NotificationSettingModel>
                {
                    new NotificationSettingModel
                    {
                        Type = NotificationType.Push,
                        Event =  NotificationEvent.ResetPassword,
                        Enabled = false
                    },
                    new NotificationSettingModel
                    {
                        Type = NotificationType.SMS,
                        Event =  NotificationEvent.CommentLiked,
                        Enabled = true
                    },
                    new NotificationSettingModel
                    {
                        Type = NotificationType.Email,
                        Event =  NotificationEvent.ResetPassword,
                        Enabled = true
                    }
                }
            };

            var store = new Mock<ISettingsDataStore>();
            store.Setup(x => x.FindAsync(It.Is<string>(s => s == userId))).ReturnsAsync(currentSettings);

            var service = new SettingsService(store.Object, DefaultMapper);

            var exception = await Assert.ThrowsAsync<ValidationException>(() => service.UpdateNotificationSettingsAsync(request));

            store.Verify(x => x.UpdateAsync(It.IsAny<UserSettings>()), Times.Never);
            Assert.Equal(3, exception.Errors.Count);
        }

        private static void VerifyUserSettings(UserSettings settings, UserProfileResponse response)
        {
            Assert.NotNull(response);
            Assert.Equal(settings.Email, response.Email);
            Assert.Equal(settings.FirstName, response.FirstName);
            Assert.Equal(settings.Language, response.Language);
            Assert.Equal(settings.LastName, response.LastName);
            Assert.Equal(settings.PhoneNumber, response.PhoneNumber);
        }

        private static void VerifyUserSettings(UserProfileRequest request, UserProfileResponse response)
        {
            Assert.NotNull(response);
            Assert.Equal(request.Email, response.Email);
            Assert.Equal(request.FirstName, response.FirstName);
            Assert.Equal(request.Language, response.Language);
            Assert.Equal(request.LastName, response.LastName);
            Assert.Equal(request.PhoneNumber, response.PhoneNumber);
        }
    }
}

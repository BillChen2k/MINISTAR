# %% md

## MNIST with PyTorch

#
# !wget www.di.ens.fr/~lelarge/MNIST.tar.gz
# !tar -zxvf MNIST.tar.gz
# !mv MNIST/raw/* ./MNISTData
# %%

import torch
import torch.nn as nn
import torch.nn.functional as F
import json
import numpy as np

torch.manual_seed(0)

# %%

# Load raw data
with open("MNISTData/train-images-idx3-ubyte", "rb") as f:
    train_image = f.read()

with open("MNISTData/train-labels-idx1-ubyte", "rb") as f:
    train_label = f.read()

with open("MNISTData/t10k-images-idx3-ubyte", "rb") as f:
    test_image = f.read()

with open("MNISTData/t10k-labels-idx1-ubyte", "rb") as f:
    test_label = f.read()

# 读取图片数
trainCount = int.from_bytes(train_image[4:8], byteorder="big", signed=False)  # 60000
testCount = int.from_bytes(test_image[4:8], byteorder="big", signed=False)  # 10000
dimension = int.from_bytes(train_image[8:12], byteorder="big", signed=False)

firstP = int.from_bytes(train_label[8:9], byteorder="big", signed=False)

x_train = np.zeros((trainCount, dimension, dimension))
y_train = np.zeros((trainCount))
x_test = np.zeros((testCount, dimension, dimension))
y_test = np.zeros((testCount))

print("Data loaded.")

# Load train manually.

x_train = np.frombuffer(train_image[16: 16 + (dimension * dimension) * trainCount], dtype=np.ubyte).reshape(
    (trainCount, dimension, dimension))
y_train = np.frombuffer(train_label[8: 8 + trainCount], dtype=np.ubyte)
x_test = np.frombuffer(test_image[16: 16 + (dimension * dimension) * testCount], dtype=np.ubyte).reshape(
    (testCount, dimension, dimension))
y_test = np.frombuffer(test_label[8: 8 + trainCount], dtype=np.ubyte)

x_test = torch.from_numpy(x_test / 255).double()
y_test = torch.from_numpy(y_test)
x_train = torch.from_numpy(x_train / 255).double()
y_train = torch.from_numpy(y_train)


# %%

CURRENT_BATCH = 0
CURRENT_EPOCH = 0

conv1_output = {
    "begin_epoch": 0,
    "begin_batch": 0,
    "end_epoch": 0,
    "end_batch": 0,
    "output": []
}

fc1_output = {
    "begin_epoch": 0,
    "begin_batch": 0,
    "end_epoch": 0,
    "end_batch": 0,
    "output": []
}

def conv1hook(module, input, output):
    return
    if len(conv1_output["output"]) == 0:
        conv1_output["begin_batch"], conv1_output["begin_epoch"] = CURRENT_BATCH, CURRENT_EPOCH
    conv1_output["output"].append({
        "batch": CURRENT_BATCH,
        "epoch": CURRENT_EPOCH,
        "output": tensor2list(output)
    })
    if len(conv1_output) >= 5:
        conv1_output["end_batch"], conv1_output["end_epoch"] = CURRENT_BATCH, CURRENT_EPOCH
        with open("ModelWeight/convout_e{}b{}_e{}n{}.json".format(
                conv1_output["begin_epoch"], conv1_output["begin_batch"],
                conv1_output["end_epoch"], conv1_output["end_batch"]), "w+") as f:
            json.dump(conv1_output, f, indent=1)
        conv1_output["output"] = []

def fc1hook(module, input, output):
    if len(fc1_output["output"]) == 0:
        fc1_output["begin_batch"], fc1_output["begin_epoch"] = CURRENT_BATCH, CURRENT_EPOCH
    fc1_output["output"].append({
        "batch": CURRENT_BATCH,
        "epoch": CURRENT_EPOCH,
        "output": tensor2list(output)
    })
    if len(fc1_output) >= 10:
        fc1_output["end_batch"], fc1_output["end_epoch"] = CURRENT_BATCH, CURRENT_EPOCH
        with open("ModelWeight/fcout_{}b{}_e{}n{}.json".format(
                fc1_output["begin_epoch"], fc1_output["begin_batch"],
                fc1_output["end_epoch"], fc1_output["end_batch"]), "w+") as f:
            json.dump(fc1_output, f, indent=1)
        fc1_output["output"] = []
    pass

class MNISTNet(nn.Module):

    def __init__(self):
        """
        Initialize the model.
        """
        kernel_number = 5
        super(MNISTNet, self).__init__()
        # Conv Layer, input size = (n_samples, channels, height, width)
        self.conv1 = nn.Conv2d(in_channels=1, out_channels=kernel_number, kernel_size=5)
        self.mp = nn.MaxPool2d(kernel_size=3)
        self.do = nn.Dropout(0.2)
        self.fc1 = nn.Linear(in_features=kernel_number * 8 * 8, out_features=10)

        self.conv1.register_forward_hook(conv1hook)
        self.fc1.register_forward_hook(fc1hook)


    def forward(self, x):
        x = self.conv1(x.double())
        x = self.mp(x)
        x = x.view(-1, self.num_feature_flat(x))  # Flatten into [BATCH_SIZE, 8 * 8 * 5]
        # x = self.do(x)
        # x = F.sigmoid(x)
        x = self.fc1(x)
        return F.log_softmax(x)

    @staticmethod
    def num_feature_flat(x):
        size = x.size()[1:]
        num_features = 1
        for s in size:
            num_features *= s
        return num_features


# %%

net = MNISTNet().double()

###
# net.to(device)


print(net)
params = list(net.parameters())

# %%

# Load data
# !wget www.di.ens.fr/~lelarge/MNIST.tar.gz
# !tar -zxvf MNIST.tar.gz
# from torchvision import datasets, transforms
# transform = transforms.Compose([transforms.ToTensor()])
# data_train = datasets.MNIST(root="./", train=True, download=True, transform=transform)

def tensor2list(x, digit=9):
    rounded = (x * 10**digit).round() / 10**digit
    return rounded.tolist()
    # if len(x.shape) == 1:
    #     return [round(one, digit) for one in x.tolist()]
    # else:
    #     return [tensor2list(x) for one in x]
    # return np.round(array, decimals=digit).tolist()

# %%
BATCH_SIZE = 150
EPOCH = 4
LEARNING_RATE = 0.0009

from torch.utils.data import TensorDataset, DataLoader
from torch.autograd import Variable

xy_train = TensorDataset(x_train.double(), y_train)
xy_test = TensorDataset(x_test.double(), y_test)

train_loader = DataLoader(xy_train, batch_size=BATCH_SIZE, shuffle=True)

test_loader = DataLoader(xy_test, batch_size=BATCH_SIZE, shuffle=True)

optimizer = torch.optim.SGD(net.parameters(), lr=LEARNING_RATE)

modelParams = {
    "begin_epoch": 0,
    "begin_batch": 0,
    "end_epoch": 0,
    "end_batch": 0,
    "weights": []
}

def train(epoch):
    net.train()
    total_correct = 0
    global CURRENT_EPOCH, CURRENT_BATCH
    for batch_index, (data, target) in enumerate(train_loader):
        CURRENT_EPOCH, CURRENT_BATCH = epoch, batch_index
        # data, target = data.to(device), target.to(device)
        # Turn from [150, 28, 28] into [150, 1, 28, 28]
        data = data.unsqueeze(1)
        # Auto Gradient
        data = Variable(data)
        target = Variable(target.long())
        output = net(data)
        loss = F.nll_loss(output, target)
        # loss = F.cross_entropy(output, target)
        loss.backward()
        optimizer.step()

        # Gathering Data:
        pred = output.data.max(dim=1, keepdim=True)[1]
        correct = pred.eq(target.data.view_as(pred)).sum().item()
        total_correct += correct
        accuracy = 100. * total_correct / (len(data) * (batch_index + 1))
        batch_accuracy =  100. * correct / (len(data))

        modelData = {
            "epoch": epoch,
            "batch": batch_index,
            "loss": loss.data.item(),
            "accuracy": accuracy,
            "input": tensor2list(data[0][0], digit=9),
            "output": pred[0].item(),
            "conv_layer": {
                "w": tensor2list(net.conv1.weight),
                "b":  tensor2list(net.conv1.bias)
            },
            "dense_layer": {
                "w": tensor2list(net.fc1.weight),
                "b": tensor2list(net.fc1.bias)
            }
        }

        if batch_index % 10 == 0:
            print('Epoch: {} [{}/{} ({:.5f}%)]\tLoss: {:.6f} Accuracy: {:.5f} Batch Accu. {:.5f}'.format(
                epoch, batch_index * len(data), len(train_loader.dataset), 100. * batch_index / len(train_loader),
                loss, accuracy, batch_accuracy))

        continue

        if len(modelParams["weights"]) == 0:
            modelParams["begin_batch"], modelParams["begin_epoch"] = batch_index, epoch

        modelParams["weights"].append(modelData)

        if len(modelParams["weights"]) >= 20:
            modelParams["end_batch"], modelParams["end_epoch"] = batch_index, epoch
            with open("ModelWeight/param_e{}b{}_e{}n{}.json".format(modelParams["begin_epoch"], modelParams["begin_batch"],
                                                                    modelParams["end_epoch"], modelParams["end_batch"]), "w+") as f:
                json.dump(modelParams, f, indent=1)
            modelParams["weights"] = []

def test(loader):
    with torch.no_grad():
        test_loss = 0.
        correct = 0
        for batch_index, (data, target) in enumerate(loader):
            # data, target = data.to(device), target.to(device)
            data = data.unsqueeze(1)
            output = net(data)
            pred = output.data.max(dim=1, keepdim=True)[1]
            correct += pred.eq(target.data.view_as(pred)).sum().item()
            test_loss += F.nll_loss(output.data, target.long().data).data.item()
        test_loss /= len(loader)
        print('\nTest set: Average loss: {:.5f}, Accuracy: {}/{} ({:.5f}%)\n'.format(
            test_loss, correct, len(loader.dataset),
            100. * correct / len(loader.dataset)))

for epoch in range(EPOCH):
    train(epoch)
    test(train_loader)

test(test_loader)

#%%
import cv2

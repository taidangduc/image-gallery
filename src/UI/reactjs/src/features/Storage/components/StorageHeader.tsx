import { Box, Button, Flex, Separator } from "@chakra-ui/react";
import { VscAdd, VscRefresh, VscTrash } from "react-icons/vsc";

type StorageHeaderProps = {
    onShowDrawer: () => void
}

export function StorageHeader({ onShowDrawer }: StorageHeaderProps) {

    return (
        <Box>
            <Flex gap="3" align="center" >
                <Box px="0">
                    <Button bg="none" color="black" px="2" onClick={() => onShowDrawer()}>
                        <VscAdd />Upload
                    </Button>                 
                </Box>
                <Button bg="none" color="black" ><VscRefresh />Refresh</Button>
                <Separator orientation="vertical" height="4" />
                <Button bg="none" color="black" ><VscTrash />Delete</Button>
            </Flex>
        </Box>
    )
}